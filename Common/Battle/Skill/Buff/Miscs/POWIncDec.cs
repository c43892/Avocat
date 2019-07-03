using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    // 魔力提升
    public class POWInc : OverlayAttrBuff
    {
        public POWInc(Warrior onwer, int roundNum, int overlay) : base(onwer, roundNum, overlay) { }
        public override string ID { get => "POWInc"; }
        public override string ReplaceBuffID { get => "POWDec"; }
        public override void UpdateOwnerAttrs() => Owner.POWInc += Overlays * 15;
    }

    // 魔力下降
    public class POWDec : OverlayAttrBuff
    {
        public POWDec(Warrior onwer, int roundNum, int overlay) : base(onwer, roundNum, overlay) { }
        public override string ID { get => "POWDec"; }
        public override string ReplaceBuffID { get => "POWInc"; }
        public override void UpdateOwnerAttrs() => Owner.POWInc -= Overlays * 15;
    }
}
