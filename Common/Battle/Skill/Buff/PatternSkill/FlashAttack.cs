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
    public class FlashAttack : PatternSkill, ISkillWithAXY
    {
        public override string Name => "FlashAttack";
        public override string DisplayName => "一闪";

        // 触发模式
        public string[] Pattern { get; set; } = new string[] { "ATK", "ES" };
        public override string[] CardsPattern { get => Pattern; }

        // 寻找附近目标攻击
        public override void Fire()
        {
            var target = Map.FindNearestTarget(Owner);
            BT.Attack(Owner, target, this, "ExtraAttack", "SuppressCounterAttack","SkillAttack");
        }

        public override string SkillDescription { get { return "这是一闪的技能描述这是一闪的技能描述这是一闪的技能描述这是一闪的技能描述"; } }

        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
