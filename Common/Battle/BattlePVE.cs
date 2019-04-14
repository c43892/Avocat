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

        public static readonly int PlayerIndex = 1; // 玩家是 1，机器人是 2

        // 玩家当前可用战斗卡牌，每回合重新生成
        public List<BattleCard> AvailableCards = new List<BattleCard>();

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

        void ResetAvailableCards(BattleCard[] cards = null)
        {
            if (cards != null)
            {
                AvailableCards.Clear();
                AvailableCards.AddRange(cards);
            }

            var moveRange = AvailableCards.Count;
            Map.ForeachWarriors((x, y, warrior) =>
            {
                if (warrior.Owner == PlayerIndex)
                    warrior.MoveRange = moveRange;
            });
        }

        // 创建 PVE 战斗逻辑
        void BuildLogic()
        {
            ConsumeCardsOnMoving().BattleStatusTransfer(1, 2).ResetDefenceAtRoundStart().AutoGenBattleCards((player) => player == 1 ? 6 : 0, // 只有玩家需要生成卡牌
                (player, cards) =>
                {
                    if (player == PlayerIndex)
                        ResetAvailableCards(cards);
                });
        }

        // 移动消耗卡牌
        BattlePVE ConsumeCardsOnMoving()
        {
            AfterMoveOnPath.Add((warrior, fx, fy, movedPath) =>
            {
                if (warrior.Owner != PlayerIndex)
                    return;

                var movedPathLen = movedPath.Count / 2;
                Debug.Assert(movedPathLen <= AvailableCards.Count, "moved path grids should not be more than cards number");

                FC.For(movedPathLen, (i) =>
                {
                    AvailableCards[0].ExecuteOn(warrior);
                    AvailableCards.RemoveAt(0);
                });

                ResetAvailableCards();
            });

            return this;
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
