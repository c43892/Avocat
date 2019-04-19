using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 游川隐
    /// 剑道，一闪
    /// </summary>
    public class YouYinChuan : Hero
    {
        public YouYinChuan(Battle bt)
            : base(bt, 10, 10)
        {
            Name = "游川隐";
            AttackRange = 2;
            ATK = 1;
        }

        protected override void SetupBuffAndSkills()
        {
            FC.Async2Sync(Battle.AddBuff(new Kendo(), this)); // 剑道
            FC.Async2Sync(Battle.AddBuff(new FlashAttack(), this)); // 一闪
        }
    }
}
