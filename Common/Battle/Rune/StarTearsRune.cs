using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 余晖
    /// 黛丽万死亡时触发一次星之泪效果
    /// </summary>
    public class StarTeasRune1 : Rune
    {
        public StarTeasRune1()
        {
            DisplayName = "余晖";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            var buff = h.GetBuff<StarsTears>();
            buff.TriggerOnDie = true;
        }
    }
}
