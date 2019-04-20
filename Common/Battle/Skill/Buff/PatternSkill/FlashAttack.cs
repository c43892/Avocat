using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 游川隐
    /// 一闪
    /// </summary>
    public class FlashAttack : PatternSkill
    {
        public override string Name => "FlashAttack";

        // 触发模式
        public override string[] CardsPattern { get; protected set; } = new string[] { "ATK", "ES" };

        // 寻找附近目标攻击
        public override IEnumerator Fire()
        {
            var target = Map.FindNearestTarget(Warrior);
            yield return BT.Attack(Warrior, target, "ExtraAttack", "SuppressCounterAttack");
        }
    }
}
