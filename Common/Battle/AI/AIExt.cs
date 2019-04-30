using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace Avocat
{
    public static class AIUtils
    {
        // 根据优先级重新排序 ai 执行顺序
        public static void Resort(this WarriorAI[] ais)
        {
            // 最近敌人的距离
            var dist2Target = new Dictionary<WarriorAI, int>();
            foreach (var ai in ais)
            {
                ai.Warrior.GetPosInMap(out int fx, out int fy);
                var target = ai.Warrior.Map.FindNearestTarget(ai.Warrior);
                if (target != null)
                {
                    target.GetPosInMap(out int tx, out int ty);
                    dist2Target[ai] = MU.ManhattanDist(fx, fy, tx, ty);
                }
            }

            // 排序权重：距离敌人距离小，攻击力高，HP 高，防御高
            ais.SwiftSort((a, b) =>
            {
                if (dist2Target[a] < dist2Target[b])
                    return -1;
                else if (dist2Target[a] > dist2Target[b])
                    return 1;
                else if (a.Warrior.BasicAttackValue > b.Warrior.BasicAttackValue)
                    return -1;
                else if (a.Warrior.BasicAttackValue < b.Warrior.BasicAttackValue)
                    return 1;
                else if (a.Warrior.HP < b.Warrior.HP)
                    return 1;
                else if (a.Warrior.HP > b.Warrior.HP)
                    return -1;
                else if (a.Warrior.HP < b.Warrior.HP)
                    return 1;
                else if (a.Warrior.GetEstimatedDefence() > b.Warrior.GetEstimatedDefence())
                    return -1;
                else if (a.Warrior.GetEstimatedDefence() < b.Warrior.GetEstimatedDefence())
                    return -1;
                else
                    return 0;
            });
        }

        public static WarriorAI Build(this WarriorAI ai, string aiType)
        {
            switch (aiType)
            {
                case "NormalNpcMonster":
                    ai.ActLast = () => NormalNpcMonster(ai);
                    break;
                case "EMPConnon":
                    ai.ActLast = () => CombineAIs(StraightForwardAndAttack(ai), AddHpRoundly(ai, (warrior) => -((int)(warrior.MaxHP * 0.4f)).Clamp(1, warrior.HP)));
                    break;
                case "Dumb":
                    break;
                default:
                    Debug.Assert(false, "no such type of ai: " + aiType);
                    break;
            }

            return ai;
        }

        #region 基本组成功能

        // 哑 AI，不做任何事情
        static IEnumerator Dumb(WarriorAI ai)
        {
            yield return null;
        }

        // 走向最近的目标并攻击之
        static IEnumerator StraightForwardAndAttack(WarriorAI ai)
        {
            // 先寻找最近目标
            var warrior = ai.Warrior;
            var target = warrior.Map.FindNearestTarget(warrior);
            if (warrior.ActionDone || target == null)
                yield break;

            yield return ForwardAndAttack(ai, target);
        }

        // 指向指定目标
        static IEnumerator Forward2Target(WarriorAI ai, Warrior target)
        {
            var warrior = ai.Warrior;
            var bt = warrior.Map.Battle;
            target.GetPosInMap(out int tx, out int ty);
            warrior.GetPosInMap(out int fx, out int fy);
            var path = warrior.MovingPath;
            path.Clear();
            path.AddRange(warrior.Map.FindPath(fx, fy, tx, ty, warrior.MoveRange));

            // 限制移动距离
            while (path.Count > warrior.MoveRange * 2)
                path.RemoveRange(path.Count - 2, 2);

            if (path.Count > 0)
                yield return bt.MoveOnPath(warrior);
        }

        // 走向指定目标，并攻击之
        static IEnumerator ForwardAndAttack(WarriorAI ai, Warrior target)
        {
            var warrior = ai.Warrior;
            var bt = warrior.Map.Battle;

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制

            // 不在攻击范围内，则先移动过去
            if (!warrior.InAttackRange(tx, ty) && warrior.MoveRange > 0)
                yield return Forward2Target(ai, target);

            // 攻击目标
            if (warrior.InAttackRange(tx, ty))
                yield return bt.Attack(warrior, target);
        }

        // 普通怪物 npc 战斗逻辑
        static IEnumerator NormalNpcMonster(WarriorAI ai)
        {
            var warrior = ai.Warrior;
            var bt = warrior.Map.Battle;
            var map = warrior.Map;

            // 检查在移动后可以攻击到的敌人
            Warrior target = null;
            yield return FindPriorTarget(warrior, FindTargetsReachable(warrior), (t) => target = t);
            if (target == null)
            {
                // 没有够得到的攻击目标
                if (warrior.AttackRange.Length == 1 && warrior.AttackRange[0] == 1) // 近战单位
                {
                    // 近战单位，就寻找血量最少的目标，向其移动
                    target = FindTheWeakestTarget(warrior);
                    yield return ForwardAndAttack(ai, target);
                }
                else
                {
                    // 远程单位，就寻找最近的队友，向其移动
                    target = FindTheNearestTeammate(warrior);
                    if (target != null)
                        yield return Forward2Target(ai, target);
                }
            }
            else
            {
                // 根据要攻击的目标确定站位
                // CheckoutAttackingPosition(warrior, target, out int tx, out int ty);
            }
        }

        // 组合任意多个 ai
        static IEnumerator CombineAIs(params IEnumerator[] ais)
        {
            for (var i = 0; i < ais.Length; i++)
                yield return ais[i];
        }
        
        // 每回合加血
        static IEnumerator AddHpRoundly(WarriorAI ai, Func<Warrior, int> calcDhp)
        {
            var warrior = ai.Warrior;
            var bt = warrior.Battle;
            var dhp = calcDhp(warrior);
            yield return bt.AddHP(warrior, dhp);
        }

        #endregion

        // 计算给定角色移动后，可以攻击到哪些目标
        static Warrior[] FindTargetsReachable(Warrior warrior)
        {
            var targets = new List<Warrior>(); // 潜在可以攻击到的目标
            var map = warrior.Map;
            warrior.GetPosInMap(out int fx, out int fy);
            map.ForeachWarriors((tx, ty, target) =>
            {
                if (warrior.Team == target.Team) // 过滤掉队友
                    return;

                var dstX = fx;
                var dstY = fy;

                if (warrior.MoveRange > 0)
                {
                    // 向目标寻路，如果无法达到，返回的路径表示朝向目标最近的方向的移动路径
                    var path2Target = map.FindPath(fx, fy, tx, ty, warrior.MoveRange);
                    if (path2Target.Count > 0)
                    {
                        dstX = path2Target[path2Target.Count - 2];
                        dstY = path2Target[path2Target.Count - 1];
                    }
                }

                // 如果寻路结果的终点，可以攻击到目标，则加入候选列表
                if (warrior.AttackRange.IndexOf(MU.ManhattanDist(dstX, dstY, tx, ty)) >= 0)
                    targets.Add(target);
            });

            return targets.ToArray();
        }

        // 寻找血量最少的目标
        public static Warrior FindTheWeakestTarget(Warrior warrior)
        {
            Warrior weakestTarget = null;
            var map = warrior.Map;
            map.ForeachWarriors((tx, ty, target) =>
            {
                if (warrior.Team == target.Team) // 过滤掉队友
                    return;

                if ((weakestTarget == null || weakestTarget.HP > target.HP) // 选血量少的
                    || (weakestTarget.HP == target.HP && weakestTarget.ES > target.ES) // 血量相同选护盾少的
                    || (weakestTarget.HP == target.HP && weakestTarget.ES == target.ES
                    && (weakestTarget.GetEstimatedDefence(warrior.AttackingType) > target.GetEstimatedDefence(warrior.AttackingType)))) // 血量护盾相同选防低的
                {
                    weakestTarget = target;
                }
            });

            return weakestTarget;
        }

        // 寻找最近的队友
        public static Warrior FindTheNearestTeammate(Warrior warrior)
        {
            Warrior nearestTeammate = null;
            var map = warrior.Map;
            warrior.GetPosInMap(out int fx, out int fy);
            var nearestX = 0;
            var nearestY = 0;
            map.ForeachWarriors((tx, ty, target) =>
            {
                if (warrior.Team != target.Team || warrior == target) // 过滤掉敌人和自己
                    return;

                if (// 选距离最近的
                    (nearestTeammate == null || MU.ManhattanDist(fx, fy, tx, ty) < MU.ManhattanDist(fx, fy, nearestX, nearestY))
                    // 距离相同选 HP 高的
                    || (MU.ManhattanDist(fx, fy, tx, ty) == MU.ManhattanDist(fx, fy, nearestX, nearestY) && nearestTeammate.HP < target.HP)
                    // 距离，HP 相同选防御高的
                    || (MU.ManhattanDist(fx, fy, tx, ty) == MU.ManhattanDist(fx, fy, nearestX, nearestY)
                    && nearestTeammate.HP == target.HP && nearestTeammate.GetEstimatedDefence() < target.GetEstimatedDefence())
                    // 距离，HP，防御相同选攻击高的
                    || (MU.ManhattanDist(fx, fy, tx, ty) == MU.ManhattanDist(fx, fy, nearestX, nearestY)
                    && nearestTeammate.HP == target.HP && nearestTeammate.GetEstimatedDefence() == target.GetEstimatedDefence()
                    && (nearestTeammate.BasicAttackValue < target.BasicAttackValue)))
                {
                    nearestX = tx;
                    nearestY = ty;
                    nearestTeammate = target;
                }
            });

            return nearestTeammate;
        }

        // 从多个攻击目标中，挑选一个最优先
        public static IEnumerator FindPriorTarget(Warrior warrior, Warrior[] targets, Action<Warrior> onSelTarget)
        {
            if (targets.Length == 0)
            {
                onSelTarget(null);
                yield break;
            }
            else if (targets.Length == 1)
            {
                onSelTarget(targets[0]);
                yield break;
            }

            // 对目标进行评分
            var priorityScore = new Dictionary<Warrior, int>();
            foreach (var t in targets)
                yield return GetTargetPriorityScore(warrior, t, (score) => priorityScore[t] = score);

            // 防御最低的目标额外 2 分
            Warrior lowestDefenceOne = null;
            foreach (var t in targets)
            {
                if (lowestDefenceOne == null 
                    || lowestDefenceOne.GetEstimatedDefence(warrior.AttackingType) > t.GetEstimatedDefence(warrior.AttackingType))
                    lowestDefenceOne = t;
            }
            priorityScore[lowestDefenceOne] += 2;

            // 攻击最低的目标额外 1 分
            Warrior lowestAttackOne = null;
            foreach (var t in targets)
            {
                if (lowestAttackOne == null || lowestAttackOne.BasicAttackValue > t.BasicAttackValue)
                    lowestAttackOne = t;
            }
            priorityScore[lowestAttackOne] += 1;

            targets.SwiftSort((a, b) => priorityScore[b] - priorityScore[a]);
            onSelTarget(targets[0]);
        }

        // 计算目标优先级分数
        public static IEnumerator GetTargetPriorityScore(Warrior warrior, Warrior target, Action<int> onScore)
        {
            var score = 0;

            // 不能反击，5 分
            if (target.GetActiveSkillByName("CounterAttack") == null)
                score += 5;

            // 能击杀的，10 分
            var damage = 0;
            yield return warrior.Battle.SimulateAttackingDamage(warrior, target, null, (d) => damage = d);
            if (damage >= target.HP + target.ES)
                score += 10;

            // 无护盾的目标，3 分
            if (target.ES <= 0)
                score += 3;

            // 血量 < 50% ，3 分
            if (target.HP * 2 < target.MaxES)
                score += 3;
            else if (target.HP * 4 < target.MaxES * 3) // 血量 < 75%，1 分
                score += 1;

            // 没有相邻队友，2 分
            if (CheckTeammateInDistance(warrior, 1).Length == 0)
                score += 2;

            // 被围攻，3 分
            if (CheckThreatenedEnemies(warrior).Length > 1)
                score += 3;

            onScore(score);
        }

        // 检查指定范围内有多少队友
        public static Warrior[] CheckTeammateInDistance(Warrior warrior, int dist)
        {
            var map = warrior.Map;
            warrior.GetPosInMap(out int cx, out int cy);
            var teammates = new List<Warrior>();
            FC.RectForCenterAt(cx, cy, dist, dist, (x, y) =>
            {
                if (!MU.InRect(cx, cy, 0, 0, map.Width, map.Height))
                    return;

                var t = map.GetWarriorAt(x, y);
                if (t != null && t.Team == warrior.Team)
                    teammates.Add(t);
            });

            return teammates.ToArray();
        }

        // 检查有多少敌人可以攻击到指定目标
        public static Warrior[] CheckThreatenedEnemies(Warrior target)
        {
            var map = target.Map;
            target.GetPosInMap(out int cx, out int cy);
            var enemies = new List<Warrior>();
            map.ForeachWarriors((x, y, e) =>
            {
                if (e.InAttackRange(cx, cy))
                    enemies.Add(e);
            });

            return enemies.ToArray();
        }

        //// 根据攻击者和目标确定攻击站位
        //public static void CheckoutAttackingPosition(Warrior attacker, Warrior target, out int tx, out int ty)
        //{

        //}
    }
}
