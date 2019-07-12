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
    public class AddConcentrateOnCriticalBeforeAction : BuffWithOwner
    {
        public override string ID { get => "AddConcentrateOnCriticalBeforeAction"; }
        public AddConcentrateOnCriticalBeforeAction(Warrior owner) : base(owner) { }

        void OnBeforeStartNextRound(int team)
        {
            if (team != Owner.Team)
                return;

            Battle.AddBuff(new ConcentrateOnCritical(Owner, 2));
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound1 += OnBeforeStartNextRound;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeStartNextRound1 -= OnBeforeStartNextRound;
            base.OnDetached();
        }
    }
}
