using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 游川隐
    /// 创伤，攻击时施加不可治疗效果
    /// </summary>
    public class PutOnUntreatableWhenAttack : PassiveSkill
    {
        IEnumerator OnAfterAttack(Warrior attacker, Warrior target)
        {
            if (attacker != Target)
                yield break;

            yield return Battle.AddBuff(new Untreatable(2), target);
        }

        public override IEnumerator OnAttached()
        {
            Battle.AfterAttack.Add(OnAfterAttack);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.AfterAttack.Del(OnAfterAttack);
            yield return base.OnDetached();
        }
    }

    /// <summary>
    /// 游川隐
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
            if (warrior != Target)
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
