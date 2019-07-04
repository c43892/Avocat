using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 洛里斯
    /// EMP 炮台
    /// </summary>
    public class EMPCannon : Npc
    {
        public EMPCannon(BattleMap map, bool fastCannon)
            : base(map)
        {
            ID = "EMPCannon";
            AI = new WarriorAI(this).Build(fastCannon ? "FastEMPConnon" : "EMPConnon");
        }
    }
}
