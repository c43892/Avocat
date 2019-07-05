using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 黛丽万
    /// 星之泪 蝶舞
    /// </summary>
    public class DaiLiWan : Hero
    {
        public DaiLiWan(Battle bt)
            : base(bt)
        {
            ID = "DaiLiWan";
            SetupSkills(new ButterflyAOE(this), new StarsTears(this), new CounterAttack(this));
        }
    }
}
