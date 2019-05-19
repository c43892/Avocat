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
    public class CannonEMP : Npc
    {
        public CannonEMP(BattleMap map)
            : base(map)
        {
            DisplayName = "EMP 炮";
            Name = "EMPCannon";
            AI = new WarriorAI(this).Build("EMPConnon");
        }
    }
}
