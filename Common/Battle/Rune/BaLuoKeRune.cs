using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 不动如山
    /// 快速援护会使巴洛克获得满额护盾；巴洛克始终处于近战形态
    /// </summary>
    public class BaLuoKeRune1 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as BaLuoKe;
            Debug.Assert(h != null, "only available for BaLuoKe");

            h.ReplaceActiveSkill(Configuration.Config(new FastAssistance2(h)));
            h.State = "Lancer";
        }
    }

    /// <summary>
    /// 背水一战
    /// 巴洛克不再提供任何指令卡；巴洛克在行动阶段前赋予全体友方单位攻击提升与魔力提升各一层
    /// </summary>
    public class BaLuoKeRune2 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as BaLuoKe;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.GetPassiveSkill<TacticalCommand>().Impl = new TacticalCommandImpl2();
        }
    }

    /// <summary>
    /// 侵略如火
    /// 当巴洛克拥有3层魔力提升时，我方全体单位获得[攻击时，封锁对手反击]
    /// </summary>
    public class BaLuoKeRune3 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as BaLuoKe;
            Debug.Assert(h != null, "only available for BaLuoKe");

            h.Battle.AddBuff(new AllSuppressCounterAttackOn3POWInc(h));
        }
    }

    /// <summary>
    /// 全能指挥
    /// 无论处于何种形态，巴洛克都会提供攻击指令与充能指令各1张
    /// </summary>
    public class BaLuoKeRune4 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as BaLuoKe;
            Debug.Assert(h != null, "only available for LuoLiSi");

            var impl = h.GetPassiveSkill<TacticalCommand>().Impl;
            if (impl is TacticalCommandImpl1)
            {
                var cards = new string[] { "EN", "ATK" };
                (impl as TacticalCommandImpl1).GetCardTypes = () => cards;
            }
        }
    }
}
