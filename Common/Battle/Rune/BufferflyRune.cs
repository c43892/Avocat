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
        public override string DisplayName { get => "蝶影"; }
        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            h.ReplaceActiveSkill(new ButterflySingle());
        }
    }

    /// <summary>
    /// 天之河
    /// 释放蝶舞后，立即获得一张回复指令卡
    /// </summary>
    public class ButterflyRune2 : Rune
    {
        public override string DisplayName { get => "天之河"; }
        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            h.GetActiveSkill<Butterfly>().AddOnePTCard = true;
        }
    }
}
