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
                ai.Owner.GetPosInMap(out int fx, out int fy);
                var target = ai.Owner.Map.FindNearestTarget(ai.Owner);
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
                else if (a.Owner.BasicAttackValue > b.Owner.BasicAttackValue)
                    return -1;
                else if (a.Owner.BasicAttackValue < b.Owner.BasicAttackValue)
                    return 1;
                else if (a.Owner.HP < b.Owner.HP)
                    return 1;
                else if (a.Owner.HP > b.Owner.HP)
                    return -1;
                else if (a.Owner.HP < b.Owner.HP)
                    return 1;
                else if (a.Owner.GetEstimatedDefence() > b.Owner.GetEstimatedDefence())
                    return -1;
                else if (a.Owner.GetEstimatedDefence() < b.Owner.GetEstimatedDefence())
                    return -1;
                else
                    return 0;
            });
        }

        // 是否能够反击
        public static bool CanAttackBack(this Warrior warrior, Warrior attacker)
        {
            // 检查反击技能
            var s = warrior.GetActiveSkillByName("CounterAttack");
            if (s == null)
                return false;

            // 检查攻击范围
            attacker.GetPosInMap(out int x, out int y);
            if (!warrior.InAttackRange(x, y))
                return false;

            //// 检查它限制行动的被动技能
            //if (warrior.GetBuffSkill<Faint>() != null)
            //    return false;

            return true;
        }

        // 是否是近战单位
        public static bool IsCloseAttack(this Warrior warrior)
        {
            return warrior.AttackRange.Length == 1 && warrior.AttackRange[0] == 1;
        }

        public static WarriorAI Build(this WarriorAI ai, string aiType)
        {
            switch (aiType)
            {
                case "NormalNpcMonster":
                    ai.ActLast = () => NormalNpcMonster(ai);
                    break;
                case "EMPConnon":
                    ai.ActLast = () => {
                        NormalNpcMonster(ai); // 每次损失 40% 最大血量
                        AddHpRoundly(ai, (warrior) => -(warrior.MaxHP * 4 / 10).Clamp(1, warrior.HP));
                    };
                    break;
                case "FastEMPConnon": // 加速炮台
                    ai.ActLast = () => {
                        NormalNpcMonster(ai);
                        ai.Owner.ActionDone = false; // 额外一次攻击，每次损失 50% 最大血量
                        NormalNpcMonster(ai);
                        AddHpRoundly(ai, (warrior) => -(warrior.MaxHP / 2).Clamp(1, warrior.HP));
                    };
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
        static void Forward2NearestTargetAndAttack(WarriorAI ai)
        {
            // 先寻找最近目标
            var warrior = ai.Owner;
            var target = warrior.Map.FindNearestTarget(warrior);
            if (warrior.ActionDone || target == null)
                return;

            ForwardAndAttack(ai, target);
        }

        // 走向指定目标
        static void Forward2Target(WarriorAI ai, Warrior target)
        {
            var warrior = ai.Owner;
            var bt = warrior.Map.Battle;
            target.GetPosInMap(out int tx, out int ty);
            warrior.GetPosInMap(out int fx, out int fy);
            var path = warrior.MovingPath;
            path.Clear();
            path.AddRange(warrior.Map.FindPath(fx, fy, tx, ty, warrior.MoveRange, warrior.StandableTiles));

            // 限制移动距离
            while (path.Count > warrior.MoveRange * 2)
                path.RemoveRange(path.Count - 2, 2);

            if (path.Count > 0)
                bt.MoveOnPath(warrior);
        }

        // 走向指定目标，并攻击之
        static void ForwardAndAttack(WarriorAI ai, Warrior target)
        {
            var warrior = ai.Owner;
            var bt = warrior.Map.Battle;

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制

            // 不在攻击范围内，则先移动过去
            if (!warrior.InAttackRange(tx, ty) && warrior.MoveRange > 0)
                Forward2Target(ai, target);

            // 攻击目标
            if (warrior.InAttackRange(tx, ty))
                bt.Attack(warrior, target);
        }

        // 普通怪物 npc 战斗逻辑
        static void NormalNpcMonster(WarriorAI ai)
        {
            var warrior = ai.Owner;
            var bt = warrior.Map.Battle;
            var map = warrior.Map;

            // 检查在移动后可以攻击到的敌人
            Warrior target = null;
            FindPriorTarget(warrior, FindTargetsReachable(warrior).Keys.ToArray(), (t) => target = t);
            if (target == null)
            {
                // 没有够得到的攻击目标
                if (warrior.IsCloseAttack()) // 近战单位
                {
                    // 近战单位，就寻找血量最少的目标，向其移动
                    target = FindTheWeakestTarget(warrior);
                    ForwardAndAttack(ai, target);
                }
                else
                {
                    // 远程单位，就寻找最近的队友，向其移动
                    target = FindTheNearestTeammate(warrior);
                    if (target != null)
                        Forward2Target(ai, target);
                }
            }
            else
            {
                // 根据要攻击的目标确定站位
                var tx = 0;
                var ty = 0;
                CheckoutAttackingPosition(warrior, target, (x, y) => { tx = x; ty = y; });

                warrior.GetPosInMap(out int fx, out int fy);
                var path = map.FindPath(fx, fy, tx, ty, warrior.MoveRange, warrior.StandableTiles);
                if (path.Count > 0)
                {
                    warrior.MovingPath.Clear();
                    warrior.MovingPath.AddRange(path);
                    bt.MoveOnPath(warrior);
                }
                bt.Attack(warrior, target);
            }
        }
        
        // 每回合加血
        static void AddHpRoundly(WarriorAI ai, Func<Warrior, int> calcDhp)
        {
            var warrior = ai.Owner;
            var bt = warrior.Battle;
            var dhp = calcDhp(warrior);
            bt.AddHP(warrior, dhp);
        }

        #endregion

        // 计算给定角色移动后，可以攻击到哪些目标
        static Dictionary<Warrior, KeyValuePair<int, int>> FindTargetsReachable(Warrior warrior)
        {
            var targets = new Dictionary<Warrior, KeyValuePair<int, int>>(); // 潜在可以攻击到的目标，及对应的站位
            var map = warrior.Map;
            warrior.GetPosInMap(out int fx, out int fy);
            map.ForeachObjs<Warrior>((tx, ty, target) =>
            {
                if (warrior.Team == target.Team) // 过滤掉队友
                    return;

                var dstX = fx;
                var dstY = fy;

                // 如果寻路结果的终点，可以攻击到目标，则加入候选列表
                if (warrior.AttackRange.IndexOf(MU.ManhattanDist(dstX, dstY, tx, ty)) >= 0)
                    targets[target] = new KeyValuePair<int, int>(dstX, dstY);
                else if (warrior.MoveRange > 0)
                {
                    // 向目标寻路，如果无法达到，返回的路径表示朝向目标最近的方向的移动路径
                    var path2Target = map.FindPath(fx, fy, tx, ty, warrior.MoveRange, warrior.StandableTiles);
                    for (var i = 0; i < path2Target.Count; i += 2)
                    {
                        dstX = path2Target[path2Target.Count - i - 2];
                        dstY = path2Target[path2Target.Count - i - 1];

                        // 如果寻路结果的终点，可以攻击到目标，则加入候选列表
                        if (warrior.AttackRange.IndexOf(MU.ManhattanDist(dstX, dstY, tx, ty)) >= 0)
                        {
                            targets[target] = new KeyValuePair<int, int>(dstX, dstY);
                            break;
                        }
                    }
                }
            });

            return targets;
        }

        // 寻找血量最少的目标
        public static Warrior FindTheWeakestTarget(Warrior warrior)
        {
            Warrior weakestTarget = null;
            var map = warrior.Map;
            map.ForeachObjs<Warrior>((tx, ty, target) =>
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
            map.ForeachObjs<Warrior>((tx, ty, target) =>
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
        public static void FindPriorTarget(Warrior warrior, Warrior[] targets, Action<Warrior> onSelTarget)
        {
            if (targets.Length == 0)
            {
                onSelTarget(null);
                return;
            }
            else if (targets.Length == 1)
            {
                onSelTarget(targets[0]);
                return;
            }

            // 对目标进行评分
            var priorityScore = new Dictionary<Warrior, int>();
            foreach (var t in targets)
                GetTargetPriorityScore(warrior, t, (score) => priorityScore[t] = score);

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
        public static void GetTargetPriorityScore(Warrior warrior, Warrior target, Action<int> onScore)
        {
            var score = 0;

            // 不能反击，5 分
            if (!target.CanAttackBack(warrior))
                score += 5;

            // 能击杀的，10 分
            var damage = 0;
            warrior.Battle.SimulateAttackingDamage(warrior, target, null, null, (d) => damage = d);
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
                if (!MU.InRect(x, y, 0, 0, map.Width, map.Height))
                    return;

                var t = map.GetAt<Warrior>(x, y);
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
            map.ForeachObjs<Warrior>((x, y, e) =>
            {
                if (e.InAttackRange(cx, cy))
                    enemies.Add(e);
            });

            return enemies.ToArray();
        }

        // 根据攻击者和目标确定攻击站位
        public static void CheckoutAttackingPosition(Warrior attacker, Warrior target, Action<int, int> onStandPos)
        {
            if (attacker.IsCloseAttack())
                CheckoutPosition4CloseAttacking(attacker, target, onStandPos);
            else
                CheckoutPosition4FarAttacking(attacker, target, onStandPos);
        }

        // 获取可以攻击到目标的所有站位列表
        public static Dictionary<KeyValuePair<int, int>, List<Warrior>> GetPosListWithTargetReachable(Warrior attacker, Warrior target)
        {
            // 计算哪些位置可以攻击到指定目标，并保留这些位置，及在这些位置上可以攻击到的目标列表

            var targetWithStandPos = FindTargetsReachable(attacker);
            var standPos2Targets = new Dictionary<KeyValuePair<int, int>, List<Warrior>>();

            foreach (var t in targetWithStandPos.Keys)
            {
                var pos = targetWithStandPos[t];
                if (!standPos2Targets.ContainsKey(pos))
                    standPos2Targets[pos] = new List<Warrior>();

                standPos2Targets[pos].Add(t);
            }

            // 可攻击目标中不包含指定目标的站位，不需要考虑
            foreach (var pos in standPos2Targets.Keys.ToArray())
            {
                if (standPos2Targets[pos].IndexOf(target) < 0)
                    standPos2Targets.Remove(pos);
            }

            return standPos2Targets;
        }

        // 近战攻击者站位逻辑
        public static void CheckoutPosition4CloseAttacking(Warrior attacker, Warrior target, Action<int, int> onStandPos)
        {
            var standPos2Targets = GetPosListWithTargetReachable(attacker, target);

            Warrior lowestDefenceOne = null;  // 防御最低的目
            Warrior lowestAttackOne = null; // 攻击最低的目标

            // 对这些位置进行评分
            var pos2Score = new Dictionary<KeyValuePair<int, int>, int>();
            foreach (var pos in standPos2Targets.Keys)
            {
                var x = pos.Key;
                var y = pos.Value;

                var score = 0;
                foreach (var t in standPos2Targets[pos])
                {
                    // 无反击能力，2 分
                    if (t.CanAttackBack(attacker))
                        score += 2;

                    // 能击杀的，3 分
                    var damage = 0;
                    attacker.Battle.SimulateAttackingDamage(attacker, t, null, null, (d) => damage = d);
                    if (damage >= target.HP + target.ES)
                        score += 10;

                    // 保留防御最低的目标
                    if (lowestDefenceOne == null
                            || lowestDefenceOne.GetEstimatedDefence(attacker.AttackingType) > t.GetEstimatedDefence(attacker.AttackingType))
                        lowestDefenceOne = t;

                    // 保留攻击最低的目标
                    if (lowestAttackOne == null || lowestAttackOne.BasicAttackValue > t.BasicAttackValue)
                        lowestAttackOne = t;
                }

                pos2Score[pos] = score;
            }

            foreach (var pos in standPos2Targets.Keys)
            {
                // 能攻击到防御最低的目标，额外 2 分
                if (standPos2Targets[pos].IndexOf(lowestDefenceOne) >= 0)
                    pos2Score[pos] += 2;

                // 能攻击到攻击最低的目标，额外 1 分
                if (standPos2Targets[pos].IndexOf(lowestAttackOne) >= 0)
                    pos2Score[pos] += 1;
            }

            var posArr = standPos2Targets.Keys.ToArray();
            posArr.SwiftSort((a, b) => pos2Score[b] - pos2Score[a]);
            onStandPos(posArr[0].Key, posArr[0].Value);
        }

        // 远程攻击者的站位逻辑
        public static void CheckoutPosition4FarAttacking(Warrior attacker, Warrior target, Action<int, int> onStandPos)
        {
            var standPos2Targets = GetPosListWithTargetReachable(attacker, target);

            // 对这些位置进行评分
            var pos2Score = new Dictionary<KeyValuePair<int, int>, int>();
            foreach (var pos in standPos2Targets.Keys)
            {
                var x = pos.Key;
                var y = pos.Value;

                var score = 0;
                foreach (var t in standPos2Targets[pos])
                {
                    // 进战敌人，2 分，远程敌人，1 分
                    score += t.IsCloseAttack() ? 2 : 1;

                    // 敌人没有反击能力，2 分
                    if (t.CanAttackBack(attacker))
                        score += 2;

                    // 能攻击到自己，并且自己无法反击，则 -2 分，能反击则 1
                    if (t.InAttackRange(x, y))
                        score += attacker.CanAttackBack(t) ? 1 : -2;
                }

                pos2Score[pos] = score;
            }

            var posArr = standPos2Targets.Keys.ToArray();
            posArr.SwiftSort((a, b) => pos2Score[b] - pos2Score[a]);
            onStandPos(posArr[0].Key, posArr[0].Value);
        }
    }
}
