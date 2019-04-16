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
        WarriorAI[] WarriorAIs { get; set; }

        public RobotPlayer(Battle battle, WarriorAI[] warriorAIs)
        {
            Battle = battle;
            WarriorAIs = warriorAIs;

            BuildLogic();
        }

        IEnumerator OnAfterStartNextRound(int player)
        {
            // 只处理机器人回合开始
            if (player != 2)
                yield break;

            for (var i = 0; i < WarriorAIs.Length; i++)
                yield return WarriorAIs[i].Act();

            yield return Battle.ActionDone(2);
        }

        void BuildLogic()
        {
            Battle.AfterStartNextRound.Add(OnAfterStartNextRound);
        }
    }
}
