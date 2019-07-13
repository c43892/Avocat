using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 反击
    /// </summary>
    public class CounterAttack : BuffWithOwner
    {
        public override string ID { get; } = "CounterAttack";
        public CounterAttack(Warrior owner) : base(owner) { }

        public int FinalDamageFact { get; set; } = 50; // 最终伤害系数百分比

        void AttackBack(Warrior attacker, Warrior target, List<Warrior> tars, Skill skill, HashSet<string> flags)
        {
            if (target != Owner || flags.Contains("SuppressCounterAttack"))
                return;

            Battle.Attack(target, attacker, this, "CounterAttack", "ExtraAttack", "SuppressCounterAttack", "SuppressPatternMatch");                                
        }

        void OnBeforeCalculateDamage1(Warrior attacker, Warrior target, HashSet<string> flags, ref int inc, ref int more, List<int> multiplier)
        {
            multiplier.Add(FinalDamageFact);
        }

        public override void OnAttached()
        {
            Battle.AfterAttack += AttackBack;
            
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterAttack -= AttackBack;
            Battle.BeforeCalculateDamage1 -= OnBeforeCalculateDamage1;
            base.OnDetached();
        }
    }
}
