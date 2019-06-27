using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 魔能提升
    /// </summary>
    public class POWInc : BuffCountDown
    {
        public POWInc(int num) : base(num) { }

        public override string Name { get => "POWInc"; }

        public override void OnAttached()
        {
            Owner.Battle.RemoveBuffByName(Owner, "POWDec"); // 互斥 buff
            Owner.ATKInc += 15;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Owner.ATKInc -= 15;
            base.OnDetached();
        }
    }

    /// <summary>
    /// 魔能下降
    /// </summary>
    public class POWDec : BuffCountDown
    {
        public POWDec(int num) : base(num) { }

        public override string Name { get => "POWDec"; }

        public override void OnAttached()
        {
            Owner.Battle.RemoveBuffByName(Owner, "POWInc"); // 互斥 buff
            Owner.ATKInc -= 15;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Owner.ATKInc += 15;
            base.OnDetached();
        }
    }
}
