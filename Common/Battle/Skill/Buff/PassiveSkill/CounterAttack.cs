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
        IEnumerator AttackBack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (target != Warrior || flags.Contains("SuppressCounterAttack"))
                yield break;

            attacker.GetPosInMap(out int x, out int y);
            if (!target.InAttackRange(x, y))
                yield break;

            yield return Battle.Attack(target, attacker, this, "CounterAttack", "ExtraAttack");
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
