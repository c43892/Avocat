using Swift;
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
            : base(map)
        {
            DisplayName = "野猪";
            Name = "YeZhu";
           
            AI = new WarriorAI(this).Build("NormalNpcMonster"); // AI
            Battle.AddBuff(new CounterAttack(), this); // 反击 buff
        }
    }
}
