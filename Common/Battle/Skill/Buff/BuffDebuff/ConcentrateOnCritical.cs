using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 专注：受影响对象产生暴击
    /// </summary>
    public class ConcentrateOnCritical : BuffCountDown
    {
        public override string Name { get => "ConcentrateOnCritical"; }

        public ConcentrateOnCritical(int num) : base(num) { }

        void OnBeforeAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (!flags.Contains("CriticalAttack"))
                flags.Add("CriticalAttack");
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
