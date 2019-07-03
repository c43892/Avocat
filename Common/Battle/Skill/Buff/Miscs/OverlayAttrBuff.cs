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
    public abstract class OverlayAttrBuff : CountDownBuffWithOwner, ISkillWithOverlays
    {
        public int MaxOverlays { get => 3; }
        public abstract string ReplaceBuffID { get; }
        public abstract void UpdateOwnerAttrs(); // 更新效果数值

        public int Overlays
        {
            get
            {
                return overlays;
            }
            set
            {
                overlays = value;
                UpdateOwnerAttrs();
            }
        } int overlays;

        public OverlayAttrBuff(Warrior onwer, int roundNum, int overlayNum) : base(onwer, roundNum)
        {
            overlays = overlayNum;
        }

        public override void OnAttached()
        {
            Battle.RemoveBuffByID(Owner, ReplaceBuffID); // 互斥 buff
            UpdateOwnerAttrs();
            base.OnAttached();
        }

        public override void OnDetached()
        {
            UpdateOwnerAttrs();
            base.OnDetached();
        }
    }
}
