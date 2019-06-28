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

    /// <summary>
    /// 全能指挥
    /// 无论处于何种形态，巴洛克都会提供攻击指令与充能指令各1张
    /// </summary>
    public class TacticalCommandRune3 : Rune
    {
        public TacticalCommandRune3()
        {
            DisplayName = "全能指挥";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as BaLuoKe;
            Debug.Assert(h != null, "only available for LuoLiSi");

            var impl = h.GetBuff<TacticalCommand>().Impl;
            if (impl is TacticalCommandImpl1)
            {
                var cards = new string[] { "EN", "ATK" };
                (impl as TacticalCommandImpl1).GetCardTypes = () => cards;
            }
        }
    }
}
