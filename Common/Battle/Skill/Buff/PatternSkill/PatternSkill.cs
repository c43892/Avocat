using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 卡牌模式触发技能
    /// </summary>
    public abstract class PatternSkill : Buff
    {
        public BattlePVE BT { get => Battle as BattlePVE; }

        // 触发模式
        public abstract string[] CardsPattern { get; }

        // 触发效果
        public abstract void Fire();

        // 检查卡牌模式
        public virtual void CheckPatternTrigger(Warrior warrior, List<BattleCard> cards)
        {
            if (warrior != Owner)
                return;

            var matched = false;
            for (var i = 0; i + CardsPattern.Length <= cards.Count && !matched; i++)
            {
                matched = true;
                for (var j = 0; j < CardsPattern.Length && matched; j++)
                    if (cards[i + j].Name != CardsPattern[j])
                        matched = false;
            }

            if (matched)
                Fire();
        }

        public override void OnAttached()
        {
            BT.AfterCardsConsumed += CheckPatternTrigger;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            BT.AfterCardsConsumed -= CheckPatternTrigger;
            base.OnDetached();
        }
    }
}
