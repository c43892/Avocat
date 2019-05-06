using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swift;
using Swift.AStar;

namespace Avocat
{
    /// <summary>
    /// 机器人玩家，提供 PVE 对手
    /// </summary>
    public class RobotPlayer
    {
        Battle Battle { get; set; }

        public RobotPlayer(Battle battle)
        {
            Battle = battle;
            Battle.AfterStartNextRound += OnAfterStartNextRound;
        }

        void OnAfterStartNextRound(int player)
        {
            // 只处理机器人回合开始
            if (player != 2)
                return;

            Battle.ActionDone(2);
        }
    }
}
