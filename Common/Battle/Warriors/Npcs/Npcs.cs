using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public class Npc : Warrior
    {
        public WarriorAI AI { get; set; }

        public Npc(BattleMap map, int maxHP, int maxES)
            : base(map, maxHP, maxES)
        {
        }
    }
}
