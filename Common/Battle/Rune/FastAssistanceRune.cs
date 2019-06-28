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
    public class FastAssistanceRune1 : Rune
    {
        public override string DisplayName { get => "不动如山"; }
        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as BaLuoKe;
            Debug.Assert(h != null, "only available for BaLuoKe");

            h.ReplaceActiveSkill(new FastAssistance2());
            h.State = "Lancer";
        }
    }

    /// <summary>
    /// 侵略如火
    /// 当巴洛克拥有3层魔力提升时，我方全体单位获得[攻击时，封锁对手反击]
    /// </summary>
    public class FastAssistanceRune2 : Rune
    {
        public override string DisplayName { get => "侵略如火"; }
        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as BaLuoKe;
            Debug.Assert(h != null, "only available for BaLuoKe");

            h.Battle.AddBuff(new AllSuppressCounterAttackOn3POWInc(), h);
        }
    }
}
