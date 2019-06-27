using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 背水一战
    /// 巴洛克不再提供任何指令卡；巴洛克在行动阶段前赋予全体友方单位攻击提升与魔力提升各一层
    /// </summary>
    public class TacticalCommandRune1 : Rune
    {
        public TacticalCommandRune1()
        {
            DisplayName = "背水一战";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as BaLuoKe;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.GetBuff<TacticalCommand>().Impl = new TacticalCommandImpl2();
        }
    }
}
