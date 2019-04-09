using Swift;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 普通 PVE 战斗
    /// </summary>
    public class BattlePVE : Battle
    {
        protected PlayerInfo[] Players { get; set; }
        public BattlePVE(int mapWidth, int mapHeight, int randSeed, params PlayerInfo[] players)
            :base(mapWidth, mapHeight, randSeed)
        {
            Players = players;
            BuildLogic();
        }

        bool playerPrepared = false;
        public override void PlayerPreparedImpl(int player)
        {
            Debug.Assert(!playerPrepared, "player already prepared in PVE");
            playerPrepared = true;
        }

        public override bool AllPrepared
        {
            get
            {
                return playerPrepared;
            }
        }
    }
}
