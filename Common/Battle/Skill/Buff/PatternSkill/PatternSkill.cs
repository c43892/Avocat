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
        public abstract string[] CardsPattern { get; protected set; }

        // 触发效果
        public abstract IEnumerator Fire();

        // 检查卡牌模式
        public virtual IEnumerator CheckPatternTrigger(Warrior warrior, List<BattleCard> cards)
        {
            if (warrior != Owner)
                yield break;

            var matched = false;
            for (var i = 0; i + CardsPattern.Length <= cards.Count && !matched; i++)
            {
                matched = true;
                for (var j = 0; j < CardsPattern.Length && matched; j++)
                    if (cards[i + j].Name != CardsPattern[j])
                        matched = false;
            }

            if (matched)
                yield return Fire();
        }

        public override IEnumerator OnAttached()
        {
            BT.AfterCardsConsumed.Add(CheckPatternTrigger);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            BT.AfterCardsConsumed.Del(CheckPatternTrigger);
            yield return base.OnDetached();
        }
    }
}
