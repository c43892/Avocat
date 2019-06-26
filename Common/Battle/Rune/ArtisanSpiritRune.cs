using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 匠心独运
    /// 由匠心产生的能量翻倍
    /// </summary>
    public class ArtisanSpiritRune1 : Rune
    {
        public ArtisanSpiritRune1()
        {
            DisplayName = "匠心独运";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            var s = h.GetBuff<ArtisanSpirit>();
            s.EffectFactor = 2;
        }
    }
}
