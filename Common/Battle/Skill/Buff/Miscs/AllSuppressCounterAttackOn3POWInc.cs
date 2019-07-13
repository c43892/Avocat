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
    public class AllSuppressCounterAttackOn3POWInc : BuffWithOwner
    {
        public override string ID { get => "AllSuppressCounterAttackOn3POWInc"; }
        public AllSuppressCounterAttackOn3POWInc(Warrior owner) : base(owner) { }

        void OnBeforeAttack(Warrior attacker, Warrior target, List<Warrior> tars, Skill skill, HashSet<string> flags)
        {
            var buff = Owner.GetBuffSkill<POWInc>();
            if (buff == null || buff.Num < 3)
                return;

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
