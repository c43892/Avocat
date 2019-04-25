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
            : base(bt)
        {
            Name = "洛里斯";
        }

        protected override void SetupBuffAndSkills()
        {
            FC.Async2Sync(Battle.AddBuff(new ArtisanSpirit(), this)); // 匠心
            AddActiveSkill(new DeployEMPConnon()); // 炮台
        }
    }
}
