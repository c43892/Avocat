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
        public List<WarriorAI> WarriorAIs { get; } = new List<WarriorAI>();

        public RobotPlayer(Battle battle)
        {
            Battle = battle;
            Battle.AfterStartNextRound.Add(OnAfterStartNextRound);
        }

        IEnumerator OnAfterStartNextRound(int player)
        {
            // 只处理机器人回合开始
            if (player != 2)
                yield break;

            for (var i = 0; i < WarriorAIs.Count; i++)
                yield return WarriorAIs[i].Act();

            yield return Battle.ActionDone(2);
        }
    }
}
