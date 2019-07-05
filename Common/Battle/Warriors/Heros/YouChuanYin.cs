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
            ID = "YouChuanYin";
            SetupSkills(new Kendo(this), new FlashAttack(this), new CounterAttack(this));
        }
    }
}
