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
    public abstract class PatternSkill : PassiveSkill, ISkillWithOwner
    {
        public virtual Warrior Owner { get; private set; }
        public PatternSkill(Hero owner)
        {
            Owner = owner;
            BT.AfterCardsConsumed += CheckPatternTrigger;
        }
        
        // 触发模式
        public abstract string[] CardsPattern { get; }

        public override Battle Battle { get => Owner.Battle; }
        public BattlePVE BT { get => Battle as BattlePVE; }

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
    }
}
