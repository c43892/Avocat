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
    public class DaiLiWanRune1 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            h.ReplaceActiveSkill(Configuration.Config(new ButterflySingle(h)));
        }
    }

    /// <summary>
    /// 余晖
    /// 黛丽万死亡时触发一次星之泪效果
    /// </summary>
    public class DaiLiWanRune2 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            var buff = h.GetPassiveSkill<StarsTears>();
            buff.TriggerOnDie = true;
        }
    }

    /// <summary>
    /// 星之子
    /// 黛丽万死亡时触发一次星之泪效果
    /// </summary>
    public class DaiLiWanRune3 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            var buff = h.GetPassiveSkill<StarsTears>();
            buff.WithAddtionalEffectOnSelf = true;
        }
    }

    /// <summary>
    /// 天之河
    /// 释放蝶舞后，立即获得一张回复指令卡
    /// </summary>
    public class DaiLiWanRune4 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as DaiLiWan;
            Debug.Assert(h != null, "only available for DaiLiWan");

            h.GetActiveSkill<Butterfly>().AddOnePTCard = true;
        }
    }
}
