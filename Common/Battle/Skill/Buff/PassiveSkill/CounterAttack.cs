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
    public class CounterAttack : PassiveSkill
    {
        public override string Name { get; } = "CounterAttack";

        void AttackBack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (target != Owner || flags.Contains("SuppressCounterAttack"))
                return;

            attacker.GetPosInMap(out int x, out int y);
            if (!target.InAttackRange(x, y))
                return;

             Battle.Attack(target, attacker, this, "CounterAttack", "ExtraAttack");
        }

        public override void OnAttached()
        {
            Battle.AfterAttack += AttackBack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterAttack -= AttackBack;
            base.OnDetached();
        }
    }
}
