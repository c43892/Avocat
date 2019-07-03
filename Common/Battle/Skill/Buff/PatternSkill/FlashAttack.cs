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
    public class FlashAttack : PatternSkillWithAttackTarget, ISkillWithAXY
    {
        public override string ID => "FlashAttack";

        public FlashAttack(Hero owner) : base(owner) { }

        public override string[] CardsPattern { get => Pattern; }
        public string[] Pattern { get; set; } = new string[] { "ATK", "ES" }; // 触发模式

        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // 只能攻击敌方目标
        public override bool TargetFilter(BattleMapObj target) => this.EnemyChecker(target);

        public override void FireOn(Warrior target)
        {
            BT.Attack(Owner, target, this, "SuppressCounterAttack", "SkillAttack", "SuppressPatternMatch");
        }
    }
}
