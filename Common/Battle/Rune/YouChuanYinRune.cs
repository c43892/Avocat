using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    ///// <summary>
    ///// 蝶影
    ///// 将 BufferflyAOE 替换成 Bufferfly
    ///// </summary>
    //public class YouChuanYinRune1 : Rune
    //{
    //    public override string DisplayName { get => "蝶影"; }
    //    public override void OnPreparingBattle(Hero hero)
    //    {
    //        var h = hero as DaiLiWan;
    //        Debug.Assert(h != null, "only available for DaiLiWan");

    //        h.ReplaceActiveSkill(Configuration.Config(new ButterflySingle()));
    //    }
    //}

    ///// <summary>
    ///// 余晖
    ///// 黛丽万死亡时触发一次星之泪效果
    ///// </summary>
    //public class DaiLiWanRune2 : Rune
    //{
    //    public override string DisplayName { get => "余晖"; }
    //    public override void OnPreparingBattle(Hero hero)
    //    {
    //        var h = hero as DaiLiWan;
    //        Debug.Assert(h != null, "only available for DaiLiWan");

    //        var buff = h.GetBuff<StarsTears>();
    //        buff.TriggerOnDie = true;
    //    }
    //}

    ///// <summary>
    ///// 星之子
    ///// 黛丽万死亡时触发一次星之泪效果
    ///// </summary>
    //public class DaiLiWanRune3 : Rune
    //{
    //    public override string DisplayName { get => "星之子"; }
    //    public override void OnPreparingBattle(Hero hero)
    //    {
    //        var h = hero as DaiLiWan;
    //        Debug.Assert(h != null, "only available for DaiLiWan");

    //        var buff = h.GetBuff<StarsTears>();
    //        buff.WithAddtionalEffectOnSelf = true;
    //    }
    //}

    /// <summary>
    /// 剑神
    /// 一闪的触发只需要攻击指令
    /// </summary>
    public class YouChuanYinRune4 : Rune
    {
        public override string DisplayName { get => "剑神"; }
        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as YouChuanYin;
            Debug.Assert(h != null, "only available for YouChuanYin");

            h.GetBuff<FlashAttack>().Pattern = new string[] { "ATK" };
        }
    }
}
