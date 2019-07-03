using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 攻击卡片效果
    /// </summary>
    public class CardATK : CountDownBuffWithOwner
    {
        public int ATK { get; set; } = 0;

        // 玩家效果，持续一回合 
        public CardATK(Warrior owner) : base(owner, 1) { }

        public override string ID { get; } = "CardATK";
        public override void OnAttached()
        { 
            Battle.AddATK(Owner, ATK);
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Owner.Battle.AddATK(Owner, -ATK);
            base.OnDetached();
        }
    }
}
