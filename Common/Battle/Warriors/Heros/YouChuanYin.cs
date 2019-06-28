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
    public class YouChuanYin : Hero
    {
        public YouChuanYin(Battle bt)
            : base(bt)
        {
            DisplayName = "游川隐";
            Name = "YouChuanYin";
            AddSkill(Configuration.Config(new Kendo()), Configuration.Config(new FlashAttack()));
        }
    }
}
