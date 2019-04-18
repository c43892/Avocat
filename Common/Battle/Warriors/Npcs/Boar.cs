using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 野猪
    /// 寻找最近的敌人攻击，一定概率反击
    /// </summary>
    public class Boar : Npc
    {
        public Boar(BattleMap map)
            : base(map, 10, 0)
        {
        }
    }
}
