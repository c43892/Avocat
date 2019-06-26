using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 蝶影
    /// 将 BufferflyAOE 替换成 Bufferfly
    /// </summary>
    public class ButterflyRune1 : Rune
    {
        public ButterflyRune1()
        {
            DisplayName = "蝶影";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            h.ReplaceActiveSkill(new ButterflySingle());
        }
    }
}
