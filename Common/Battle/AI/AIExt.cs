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
            var target = FindPriorTarget(warrior, FindTargetsReachable(warrior));
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
                throw new Exception("not implemented yet");
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

        // 从多个攻击目标中，挑选一个最优先
        public static Warrior FindPriorTarget(Warrior warrior, Warrior[] targets)
        {
            return targets.Length > 0 ? targets[0] : null;
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

                if (weakestTarget == null || weakestTarget.HP > target.HP)
                    weakestTarget = target;
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

                if (nearestTeammate == null || MU.ManhattanDist(fx, fy, tx, ty) < MU.ManhattanDist(fx, fy, nearestX, nearestY))
                {
                    nearestX = tx;
                    nearestY = ty;
                    nearestTeammate = target;
                }
            });

            return nearestTeammate;
        }
    }
}
