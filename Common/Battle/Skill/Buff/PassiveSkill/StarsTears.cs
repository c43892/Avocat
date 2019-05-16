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
        public override string Name { get;} = "StarsTears";
        void OnAfterAddHp(Warrior warrior, int dhp)
        {
            if (warrior.Team != Warrior.Team || warrior == Warrior || dhp <= 0)
                return;

            var bt = Battle as BattlePVE;
            var des = Calculation.StarTearsEffect(dhp);
            bt.AddES(warrior, (int)des);
        }

        public override void OnAttached()
        {
            Battle.AfterAddHP += OnAfterAddHp;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterAddHP -= OnAfterAddHp;
            base.OnDetached();
        }
    }
}
