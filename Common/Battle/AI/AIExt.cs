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
                case "Boar":
                    ai.ActFirst = () => StraightForwardAndAttack(ai, ai.Warrior.MoveRange);
                    break;
                case "EMPConnon":
                    ai.ActLast = () => CombineAIs(StraightForwardAndAttack(ai), AddHpRoundly(ai, (warrior) => -MU.Clamp((int)(warrior.MaxHP * 0.4f), 1, warrior.HP)));
                    break;
                case "Dumb":
                    ai.ActFirst = () => Dumb(ai);
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

        // 直线走向最近的目标并攻击之
        static IEnumerator StraightForwardAndAttack(WarriorAI ai, int maxMovingDist = 0)
        {
            // 先寻找最近目标
            var warrior = ai.Warrior;
            var target = warrior.Map.FindNearestTarget(warrior);
            if (target == null)
                yield break;

            var bt = warrior.Map.Battle;

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!warrior.InAttackRange(tx, ty) && maxMovingDist > 0) // 不在攻击范围内，则先移动过去
            {
                warrior.GetPosInMap(out int fx, out int fy);

                var path = warrior.MovingPath;
                path.Clear();
                path.AddRange(warrior.Map.FindPath(fx, fy, tx, ty, 1));

                // 限制移动距离
                while (path.Count > maxMovingDist * 2)
                    path.RemoveRange(path.Count - 2, 2);

                if (path.Count > 0)
                    yield return bt.MoveOnPath(warrior);
            }

            // 攻击目标
            if (warrior.InAttackRange(tx, ty))
                yield return bt.Attack(warrior, target);
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
    }
}
