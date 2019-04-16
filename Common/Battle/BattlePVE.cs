using Swift;
using System;
using System.Collections;
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

        // 暂存区的卡牌
        public List<BattleCard> StashedCards = new List<BattleCard>();
        Warrior[] Npcs { get; set; }

        // 玩家能量槽
        public int MaxEnergy { get; set; } = 100;
        public int Energy { get; set; } = 0;

        public BattlePVE(BattleMap map, int randSeed, PlayerInfo player, params Warrior[] npcs)
            :base(map, randSeed)
        {
            Player = player;
            Npcs = npcs;
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

        // 重置可用卡片
        void ResetAvailableCards(BattleCard[] cards = null)
        {
            if (cards != null)
                AvailableCards.AddRange(cards);

            var moveRange = AvailableCards.Count;
            Map.ForeachWarriors((x, y, warrior) =>
            {
                if (warrior.Owner == PlayerIndex)
                    warrior.MoveRange = moveRange;
            });
        }

        // 创建 PVE 战斗逻辑
        public void Build()
        {
            Build(1, 2);  // 1-玩家，2-机器人

            ConsumeCardsOnMoving();

            // 行动结束分解剩余卡牌
            FC.Async2Sync(AddBuff(new DisassembleCards(PlayerIndex, AvailableCards, (player, cards) => ResetAvailableCards())));

            // 行动开始前，生成新卡牌
            FC.Async2Sync(AddBuff(new GenCards(PlayerIndex,
                (player) => player == 1 ? 8 : 0,
                (player, cards) => ResetAvailableCards(cards)
            )));

            BuildRobot(Npcs);
        }

        // 移动消耗卡牌

        IEnumerator OnAfterMoveOnPath(Warrior warrior, int fx, int fy, List<int> movedPath)
        {
            if (warrior.Owner != PlayerIndex)
                yield break;

            var movedPathLen = movedPath.Count / 2;
            Debug.Assert(movedPathLen <= AvailableCards.Count, "moved path grids should not be more than cards number");

            for (var i = 0; i < movedPathLen; i++)
            {
                var card = AvailableCards[0];
                AvailableCards.RemoveAt(0);
                yield return card.ExecuteOn(warrior);
            }

            ResetAvailableCards();
        }

        BattlePVE ConsumeCardsOnMoving()
        {
            AfterMoveOnPath.Add(OnAfterMoveOnPath);
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

        // 交换战斗卡牌位置
        protected AsyncCalleeChain<int, int, int, int> BeforeBattleCardsExchange = new AsyncCalleeChain<int, int, int, int>();
        protected AsyncCalleeChain<int, int, int, int> AfterBattleCardsExchange = new AsyncCalleeChain<int, int, int, int>();
        public AsyncCalleeChain<int, int, int, int> OnBattleCardsExchange = new AsyncCalleeChain<int, int, int, int>();
        public IEnumerator ExchangeBattleCards(int g1, int n1, int g2, int n2)
        {
            yield return BeforeBattleCardsExchange?.Invoke(g1, n1, g2, n2);

            var lst1 = g1 == 0 ? AvailableCards : StashedCards;
            var lst2 = g2 == 0 ? AvailableCards : StashedCards;
            var c1 = n1 < lst1.Count ? lst1[n1] : null;
            var c2 = n2 < lst2.Count ? lst2[n2] : null;

            if (n1 < lst1.Count)
                lst1[n1] = c2;
            else
                lst1.Add(c2);

            if (n2 < lst2.Count)
                lst2[n2] = c1;
            else
                lst2.Add(c1);

            for (var i = lst1.Count - 1; i >= 0; i--)
                if (lst1[i] == null)
                    lst1.RemoveAt(i);

            for (var i = lst2.Count - 1; i >= 0; i--)
                if (lst2[i] == null)
                    lst2.RemoveAt(i);

            n1 = lst1.IndexOf(c2);
            n2 = lst2.IndexOf(c1);

            ResetAvailableCards();

            yield return BeforeBattleCardsExchange?.Invoke(g1, n1, g2, n2);

            yield return OnBattleCardsExchange?.Invoke(g1, n1, g2, n2);
        }

        #region 角色操作包装

        // 玩家加能量
        public AsyncCalleeChain<int, Action<int>> BeforeAddEN = new AsyncCalleeChain<int, Action<int>>();
        public AsyncCalleeChain<int> AfterAddEN = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnAddEN = new AsyncCalleeChain<int>();
        public IEnumerator AddEN(int den)
        {
            yield return BeforeAddEN.Invoke(den, (int _den) => den = _den);

            Energy = MU.Clamp(Energy + den, 0, MaxEnergy);

            yield return AfterAddEN.Invoke(den);
            yield return OnAddEN.Invoke(den);
        }

        #endregion
    }
}
