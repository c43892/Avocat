using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 创伤效果，不可治疗
    /// </summary>
    public class Untreatable : BuffCountDown
    {
        public Untreatable(int num)
            :base(num)
        {
        }

        IEnumerator OnBeforeAddHp(Warrior warrior, int dhp, Action<int> changeDhp)
        {
            if (warrior != Warrior)
                yield break;

            changeDhp(0);
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeAddHP.Add(OnBeforeAddHp);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeAddHP.Del(OnBeforeAddHp);
            yield return base.OnDetached();
        }
    }
}
