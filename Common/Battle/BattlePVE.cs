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
        protected PlayerInfo Player { get; set; }
        protected RobotPlayer Robot { get; set; }

        public BattlePVE(BattleMap map, int randSeed, PlayerInfo player, params Warrior[] npcs)
            :base(map, randSeed)
        {
            Player = player;
            BuildLogic();
            BuildRobot(npcs);
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

        // 创建机器人对手
        public void BuildRobot(Warrior[] npcs)
        {
            var ais = npcs.ToArray((i, npc, skipAct) =>
            {
                var ai = new WarriorAI(npc);
                ai.Build("StraightlyForwardAndAttack");
                return ai;
            });
            Robot = new RobotPlayer(this, ais);
        }
    }
}
