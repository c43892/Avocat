﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public abstract class Npc : Warrior
    {
        public Npc(BattleMap map)
            : base(map)
        {
            StandableTiles = TileType.All;
        }
    }
}
