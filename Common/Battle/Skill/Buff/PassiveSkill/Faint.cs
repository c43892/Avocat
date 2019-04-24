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
            if (warrior == Warrior)
                resetActionFlags(false, false);

            yield return null;
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeResetActionFlag.Add(UnsetActionFlag);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeResetActionFlag.Del(UnsetActionFlag);
            yield return base.OnDetached();
        }
    }
}
