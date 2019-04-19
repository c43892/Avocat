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
            : base(bt, 10, 10)
        {
            Name = "黛丽万";
            AttackRange = 3;
            ATK = 1;
        }

        protected override void SetupBuffAndSkills()
        {
            FC.Async2Sync(Battle.AddBuff(new StarsTears(), this)); // 星之泪
            AddActiveSkill(new Butterfly()); // 蝶舞
        }
    }
}
