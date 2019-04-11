using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace Avocat
{
    public static class AIUtils
    {
        public static void Build(this WarriorAI ai, string aiType)
        {
            switch (aiType)
            {
                case "StraightlyForwardAndAttack":
                    StraightlyForwardAndAttack(ai);
                    break;
                case "Dumb":
                default:
                    Dumb(ai);
                    break;
            }
        }

        // 哑 AI，不做任何事情
        static void Dumb(WarriorAI ai)
        {
        }

        // 直线走向最近的目标并攻击之
        static void StraightlyForwardAndAttack(WarriorAI ai)
        {
            ai.Act = () =>
            {
                var warrior = ai.Warrior;
                if (warrior.IsDead)
                    return;

                // 先寻找最近目标
                var target = FindNearestTarget(warrior);
                if (target == null)
                    return;

                var bt = warrior.Map.Battle;

                target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
                if (!warrior.InAttackRange(tx, ty)) // 不在攻击范围内，则先移动过去
                { 
                    warrior.GetPosInMap(out int fx, out int fy);

                    // 直接走向目标，不考虑障碍
                    var path = warrior.MovingPath;
                    path.Clear();
                    FC.ForFromTo(fx, tx, (x) => { path.Add(x); path.Add(fy); });
                    FC.ForFromTo(fy, ty, (y) => { path.Add(tx); path.Add(y); });

                    // 根据移动距离做截断，第一个点是角色当前位置，不算作移动距离
                    if (path.Count > 0)
                        path.RemoveRange(0, 2);

                    while (path.Count > warrior.MoveRange * 2)
                        path.RemoveRange(path.Count - 2, 2);

                    bt.MoveOnPath(warrior);
                }

                // 攻击目标
                if (warrior.InAttackRange(tx, ty))
                    bt.Attack(warrior, target);
            };
        }

        #region 基本组成功能

        // 寻找最近目标
        static Warrior FindNearestTarget(Warrior warrior)
        {
            Warrior nearestTarget = null;
            int tx = 0;
            int ty = 0;

            warrior.GetPosInMap(out int fx, out int fy);
            warrior.Map.ForeachWarriors((x, y, target) =>
            {
                // 过滤队友和已经死亡的敌人
                if (target.IsDead || target.Owner == warrior.Owner)
                    return;

                if (nearestTarget == null || MU.ManhattanDist(fx, fy, x, y) < MU.ManhattanDist(fx, fy, tx, ty))
                {
                    tx = x;
                    ty = y;
                    nearestTarget = target;
                }
            });

            return nearestTarget;
        }

        #endregion
    }
}
