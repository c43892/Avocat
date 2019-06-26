using System;
using System.Collections;
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
        public Warrior Owner { get; protected set; }
        public WarriorAI(Warrior warrior)
        {
            Owner = warrior;
        }

        public Action ActFirst;
        public Action ActLast;
    }
}
