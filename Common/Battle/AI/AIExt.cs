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
        public static void Build(this WarriorAI ai, string aiType)
        {
            switch (aiType)
            {
                case "StraightlyForwardAndAttack":
                    ai.Act = () => StraightlyForwardAndAttack(ai);
                    break;
                case "Dumb":
                default:
                    ai.Act = () => Dumb(ai);
                    break;
            }
        }

        // 哑 AI，不做任何事情
        static IEnumerator Dumb(WarriorAI ai)
        {
            yield return null;
        }

        // 直线走向最近的目标并攻击之
        static IEnumerator StraightlyForwardAndAttack(WarriorAI ai)
        {
            var warrior = ai.Warrior;
            if (warrior.IsDead)
                yield break;

            // 先寻找最近目标
            var target = warrior.Map.FindNearestTarget(warrior);
            if (target == null)
                yield break;

            var bt = warrior.Map.Battle;

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!warrior.InAttackRange(tx, ty)) // 不在攻击范围内，则先移动过去
            {
                warrior.GetPosInMap(out int fx, out int fy);

                var path = warrior.MovingPath;
                path.Clear();
                path.AddRange(warrior.Map.FindPath(fx, fy, tx, ty, 1));

                // 限制移动距离
                while (path.Count > warrior.MoveRange * 2)
                    path.RemoveRange(path.Count - 2, 2);

                if (path.Count > 0)
                    yield return bt.MoveOnPath(warrior);
            }

            // 攻击目标
            if (warrior.InAttackRange(tx, ty))
                yield return bt.Attack(warrior, target);
        }

        #region 基本组成功能

        #endregion
    }
}
