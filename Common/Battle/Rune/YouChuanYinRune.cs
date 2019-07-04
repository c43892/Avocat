using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 收刀术
    /// 行动阶段，游川隐在攻击后返回原来的位置
    /// </summary>
    public class YouChuanYinRune1 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as YouChuanYin;
            Debug.Assert(h != null, "only available for YouChuanYin");
            h.Battle.AddBuff(new ReturnBackAfterAttack(h));
        }
    }

    /// <summary>
    /// 拔刀术
    /// 游川隐不再提供任何指令卡；行动阶段前，游川隐获得专注
    /// </summary>
    public class YouChuanYinRune2 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as YouChuanYin;
            Debug.Assert(h != null, "only available for YouChuanYin");

            h.CardType = null;
            h.Battle.AddBuff(new ConcentrateOnCritical(h, 1));
        }
    }

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
        public override void OnPreparingBattle()
        {
            var h = Owner as YouChuanYin;
            Debug.Assert(h != null, "only available for YouChuanYin");

            h.GetBuffSkill<FlashAttack>().Pattern = new string[] { "ATK" };
        }
    }
}
