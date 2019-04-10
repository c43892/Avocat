using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 控制一个战斗角色的 AI
    /// </summary>
    public class WarriorAI
    {
        public Warrior Warrior { get; protected set; }
        public WarriorAI(Warrior warrior)
        {
            Warrior = warrior;
        }

        public Action Act;
    }
}
