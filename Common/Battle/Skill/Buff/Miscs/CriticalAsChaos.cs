using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 心眼：暴击忽略护甲
    /// </summary>
    public class CriticalAsChaos : BuffWithOwner
    {
        public override string ID { get => "CriticalWithPierce"; }
        public CriticalAsChaos(Warrior owner) : base(owner) { }

        void OnBeforeAttack(Warrior attacker, Warrior target, List<Warrior> tars, Skill skill, HashSet<string> flags, List<int> multi, List<int> addMulti)
        {
            if (flags.Contains("CriticalAttack"))
                flags.Add("chaos");
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
