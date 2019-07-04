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
            ID = "LuoLiSi";
            SetupSkills(new DeployEMPCannon(this), new ArtisanSpirit(this));
        }
    }
}
