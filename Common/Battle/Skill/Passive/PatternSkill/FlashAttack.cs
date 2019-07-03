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
    public class FlashAttack : PatternSkill, ISkillWithAXY, ISkillWithTargetFilter
    {
        public override string ID => "FlashAttack";

        public FlashAttack(Hero owner)
            : base(owner)
        {
            BT.BeforeMoveOnPathAndAttack += OnBeforeMoveOnPathAndAttack;
        }

        public override string[] CardsPattern { get => Pattern; }
        public string[] Pattern { get; set; } = new string[] { "ATK", "ES" }; // 触发模式

        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // 只能攻击敌方目标
        public bool TargetFilter(BattleMapObj target) => this.EnemyChecker(target);

        // 记录攻击目标
        Warrior tar = null;
        void OnBeforeMoveOnPathAndAttack(Warrior attacker, Warrior target)
        {
            if (attacker != Owner || !TargetFilter(target))
                return;

            tar = target;
        }

        // 寻找附近目标攻击
        public override void Fire()
        {
            BT.Attack(Owner, tar, this, "SuppressCounterAttack", "SkillAttack");
        }
    }
}
