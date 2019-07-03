using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// EMP 炮台摧毁时增加能量
    /// </summary>
    public class GainENOnEMPDestroyed : BuffWithOwner
    {
        public override string ID { get => "GainENOnEMPDestroyed"; }
        public GainENOnEMPDestroyed(Warrior owner) : base(owner) { }
        public int ES2Add { get; set; }

        void AddEN(Warrior warrior)
        {
            if (warrior is EMPCannon)
            {
                var bt = Battle as BattlePVE;
                bt.AddEN(ES2Add);
            }
        }

        public override void OnAttached()
        {
            Battle.AfterWarriorDead += AddEN;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterWarriorDead -= AddEN;
            base.OnDetached();
        }
    }
}
