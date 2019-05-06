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

        void UnsetActionFlag(Warrior warrior, Action<bool, bool> resetActionFlags)
        {
            if (warrior != Warrior)
                return;

            resetActionFlags(false, false);
        }

        void CancelAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (attacker != Warrior)
                return;

            if (!flags.Contains("CancelAttack"))
                flags.Add("CancelAttack");
        }

        public override void OnAttached()
        {
            Battle.BeforeResetActionFlag += UnsetActionFlag;
            Battle.BeforeAttack += CancelAttack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeResetActionFlag -= UnsetActionFlag;
            Battle.BeforeAttack -= CancelAttack;
            base.OnDetached();
        }
    }
}
