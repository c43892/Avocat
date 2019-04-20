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
        public CannonEMP(BattleMap map, int maxHP)
            : base(map, maxHP, 0)
        {
            Name = "EMP 炮";
            AttackRange = 1;
            AI = new WarriorAI(this).Build("EMPConnon");
        }
    }
}
