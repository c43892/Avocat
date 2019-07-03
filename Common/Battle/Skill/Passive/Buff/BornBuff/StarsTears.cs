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
    public class StarsTears : BornBuff
    {
        public override string ID { get;} = "StarsTears";
        public StarsTears(Warrior owner) : base(owner) { }

        void OnAfterAddHp(Warrior warrior, int dhp)
        {
            if (warrior.Team != Owner.Team || warrior == Owner || dhp < 0)
                return;

            var bt = Battle as BattlePVE;
            var des = Calculation.StarTearsEffect(warrior);
            bt.AddES(warrior, des);

            if (WithAddtionalEffectOnSelf)
                bt.AddES(Owner, des / 2);
        }

        void OnAfterDead(Warrior warrior)
        {
            if (!TriggerOnDie || warrior != Owner) // 自己死亡时处理逻辑
                return;

            var bt = Battle as BattlePVE;
            var des = Calculation.StarTearsEffect(warrior);
            foreach (var m in warrior.GetTeamMembers())
                bt.AddES(m, des);
        }

        // 死亡时是否对全体成员触发一次效果，这个开关会被符文打开
        public bool TriggerOnDie { get; set; } = false;

        // 是否对自身产生额外护盾效果，这个开关会被符文打开
        public bool WithAddtionalEffectOnSelf { get; set; } = false;

        public override void OnAttached()
        {
            Battle.AfterAddHP += OnAfterAddHp;
            Battle.AfterWarriorDead += OnAfterDead;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterAddHP -= OnAfterAddHp;
            Battle.AfterWarriorDead -= OnAfterDead;
            base.OnDetached();
        }
    }
}
