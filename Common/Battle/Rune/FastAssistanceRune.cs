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
    public class FastAssistanceRune1 : Rune
    {
        public FastAssistanceRune1()
        {
            DisplayName = "不动如山";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as BaLuoKe;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.ReplaceActiveSkill(new FastAssistance2());
            h.State = "Lancer";
        }
    }
}
