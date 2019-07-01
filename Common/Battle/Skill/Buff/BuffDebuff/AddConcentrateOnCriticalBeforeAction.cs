using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 每回合开始前增加专注 buff
    /// </summary>
    public class AddConcentrateOnCriticalBeforeAction : Buff
    {
        public override string Name { get => "AddConcentrateOnCriticalBeforeAction"; }

        void OnBeforeStartNextRound(int team)
        {
            if (team != Owner.Team)
                return;

            Owner.Battle.AddBuff(new ConcentrateOnCritical(2));
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += OnBeforeStartNextRound;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeStartNextRound -= OnBeforeStartNextRound;
            base.OnDetached();
        }
    }
}
