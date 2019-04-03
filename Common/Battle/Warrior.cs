using System;
using System.Collections.Generic;
using System.Text;

namespace Battle
{
    /// <summary>
    /// 战斗人员
    /// </summary>
    public class Warrior : BattleMapItem
    {
        public int Avatar { get; private set; }
        public bool IsOpponent { get; set; }
    }
}