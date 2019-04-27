using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 眩晕
    /// 没行动能力
    /// </summary>
    public class Faint : BuffCountDown
    {
        public Faint(int num)
            : base(num)
        {
        }

        IEnumerator UnsetActionFlag(Warrior warrior, Action<bool, bool> resetActionFlags)
        {
            if (warrior != Warrior)
                yield break;

            resetActionFlags(false, false);
        }

        IEnumerator CancelAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (attacker != Warrior)
                yield break;

            if (!flags.Contains("CancelAttack"))
                flags.Add("CancelAttack");
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeResetActionFlag.Add(UnsetActionFlag);
            Battle.BeforeAttack.Add(CancelAttack);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeResetActionFlag.Del(UnsetActionFlag);
            Battle.BeforeAttack.Del(CancelAttack);
            yield return base.OnDetached();
        }
    }
}
