using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 攻击提升
    /// </summary>
    public class ATKInc : BuffCountDown
    {
        public ATKInc(int num) : base(num) { }

        public override string Name { get => "ATKInc"; }

        public override void OnAttached()
        {
            Owner.Battle.RemoveBuffByName(Owner, "ATKDec"); // 互斥 buff
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
    /// 攻击下降
    /// </summary>
    public class ATKDec : BuffCountDown
    {
        public ATKDec(int num) : base(num) { }

        public override string Name { get => "ATKDec"; }

        public override void OnAttached()
        {
            Owner.Battle.RemoveBuffByName(Owner, "ATKInc"); // 互斥 buff
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
