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
            DisplayName = "洛里斯";
            Name = "LuoLiSi";
            SetupBuffAndSkills(Configuration.Config(new DeployEMPConnon()), new ArtisanSpirit());
        }
    }
}
