using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    // 攻击提升
    public class ATKInc : OverlayAttrBuff
    {
        public ATKInc(Warrior onwer, int roundNum, int overlay) : base(onwer, roundNum, overlay) { }
        public override string ID { get => "ATKInc"; }
        public override string ReplaceBuffID { get => "ATKDec"; }
        public override void UpdateOwnerAttrs() => Owner.ATKInc += Overlays * 15;
    }

    // 攻击下降
    public class ATKDec : OverlayAttrBuff
    {
        public ATKDec(Warrior onwer, int roundNum, int overlay) : base(onwer, roundNum, overlay) { }
        public override string ID { get => "ATKDec"; }
        public override string ReplaceBuffID { get => "ATKDec"; }
        public override void UpdateOwnerAttrs() => Owner.ATKInc -= Overlays * 15;
    }
}
