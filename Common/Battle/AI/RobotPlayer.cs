using System;
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

        void BuildLogic()
        {
            Battle.OnActionDone += (int player) =>
            {
                // 只处理玩家回合结束
                if (player != 1)
                    return;

                FC.ForEach(WarriorAIs, (i, ai) => ai.Act.SC());
                Battle.ActionDone(2);
            };
        }
    }
}
