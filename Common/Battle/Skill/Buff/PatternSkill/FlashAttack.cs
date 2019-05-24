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

        // 触发模式
        public override string[] CardsPattern { get; protected set; } = new string[] { "ATK", "ES" };

        // 寻找附近目标攻击
        public override void Fire()
        {
            var target = Map.FindNearestTarget(Warrior);
            BT.Attack(Warrior, target, this, "ExtraAttack", "SuppressCounterAttack","SkillAttack");
        }

        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
