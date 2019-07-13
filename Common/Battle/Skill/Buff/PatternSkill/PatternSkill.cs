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
    public abstract class PatternSkill : BuffWithOwner
    {
        public PatternSkill(Hero owner) : base(owner) { }

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

        // 触发模式
        public abstract string[] CardsPattern { get; }

        public override Battle Battle { get => Owner.Battle; }
        public BattlePVE BT { get => Battle as BattlePVE; }

        // 检查卡牌模式
        public abstract void WhenPatternMatched();
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
                WhenPatternMatched();
        }
    }

    /// <summary>
    /// 模式匹配技能并且需要攻击目标
    /// </summary>
    public abstract class PatternSkillWithAttackTarget : PatternSkill, ISkillWithTargetFilter
    {
        public PatternSkillWithAttackTarget(Hero owner) : base(owner) { }

        // 模式匹配上之后，还要等攻击目标
        bool matched = false;
        public override void WhenPatternMatched() => matched = true;

        void OnBeforeStartNextRound(int team)
        {
            if (team == Owner.Team)
                matched = false;
        }

        public override void OnAttached()
        {
            BT.BeforeStartNextRound1 += OnBeforeStartNextRound;
            BT.BeforeAttack += OnBeforeAttack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            BT.BeforeStartNextRound1 -= OnBeforeStartNextRound;
            BT.BeforeAttack -= OnBeforeAttack;
            base.OnDetached();
        }

        public virtual void FireOn(Warrior target)
        {
            matched = false;
        }

        public abstract bool TargetFilter(BattleMapObj target);

        private void OnBeforeAttack(Warrior attacker, Warrior target, List<Warrior> tars, Skill skill, HashSet<string> flags, List<int> multi, List<int> addMulti)
        {
            if (matched && TargetFilter(target) && flags.Contains("SuppressPatternMatch"))
                FireOn(target);
        }
    }
}
