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
        IEnumerator AttackBack(Warrior attacker, Warrior target, List<string> flags)
        {
            if (target != Warrior || flags.Contains("SuppressCounterAttack"))
                yield break;

            yield return Battle.Attack(target, attacker, "CounterAttack");
        }

        public override IEnumerator OnAttached()
        {
            Battle.AfterAttack.Add(AttackBack);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.AfterAttack.Del(AttackBack);
            yield return null;
        }
    }
}
