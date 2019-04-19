using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 洛里斯
    /// 匠心
    /// </summary>
    public class LuoLiSi : Hero
    {
        public LuoLiSi(Battle bt)
            : base(bt, 10, 10)
        {
            Name = "洛里斯";
            AttackRange = 1;
            ATK = 3;
        }

        protected override void SetupBuffAndSkills()
        {
            FC.Async2Sync(Battle.AddBuff(new ArtisanSpirit(), this)); // 匠心
        }
    }
}
