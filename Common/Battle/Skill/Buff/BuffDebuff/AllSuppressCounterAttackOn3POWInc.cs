using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 巴洛克
    /// 侵略如火: 当拥有 3 层魔力提升时，我方全体单位获得[攻击时，封锁对手反击]
    /// </summary>
    public class AllSuppressCounterAttackOn3POWInc : Buff
    {
        public override string Name { get => "AllSuppressCounterAttackOn3POWInc"; }

        void OnBeforeAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            var buff = Owner.GetBuff<POWInc>();
            if (buff == null || buff.Num < 3)
                return;

            if (!flags.Contains("SuppressCounterAttack"))
                flags.Add("SuppressCounterAttack");
        }

        public override void OnAttached()
        {
            Battle.BeforeAttack += OnBeforeAttack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeAttack -= OnBeforeAttack;
            base.OnDetached();
        }
    }
}
