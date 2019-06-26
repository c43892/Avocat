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
            DisplayName = "黛丽万";
            Name = "DaiLiWan";
            AddSkill(new ButterflyAOE(), new StarsTears());
        }
    }
}
