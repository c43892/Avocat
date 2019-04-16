using System;
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
        {
            Num = 1; // 一回合结束
        }

        public override void OnAttached()
        {
            Target.ATK += ATK;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Target.ATK -= ATK;
            base.OnDetached();
        }
    }
}
