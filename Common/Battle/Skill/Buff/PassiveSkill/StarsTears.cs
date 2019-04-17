using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 黛丽万
    /// 星之泪，行动阶段前，每当友方单位受到治疗时，为受到治疗的友方单位提供护盾
    /// </summary>
    public class StarsTears : PassiveSkill
    {
        IEnumerator OnAfterAddHp(Warrior warrior, int dhp)
        {
            if (warrior.Owner != Target.Owner || warrior == Target)
                yield break;

            var bt = Battle as BattlePVE;
            var des = 20 + dhp * 0.3;
            yield return bt.AddES(warrior, (int)des);
        }

        public override IEnumerator OnAttached()
        {
            Battle.AfterAddHP.Add(OnAfterAddHp);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.AfterAddHP.Del(OnAfterAddHp);
            yield return base.OnDetached();
        }
    }
}
