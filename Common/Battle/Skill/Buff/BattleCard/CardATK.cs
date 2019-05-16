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
    public class CardATK : BuffCountDown
    {
        public int ATK { get; set; } = 0;

        public CardATK()
            : base(1)  // 效果一回合结束
        {
        }
        public override string Name { get; } = "CardATK";
        public override bool isBattleBUFF { get; set; } = true;
        public override void OnAttached()
        { 
            Battle.AddATK(Warrior, ATK);
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Warrior.Battle.AddATK(Warrior, -ATK);
            base.OnDetached();
        }
    }
}
