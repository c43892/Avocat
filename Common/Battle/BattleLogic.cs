using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using System.Diagnostics;

namespace Avocat
{
    // 将复杂战斗逻辑拆分到此
    public partial class Battle
    {
        // 生成随机战斗卡牌
        BattleCard[] GenNextCards(int num)
        {
            var cards = new BattleCard[num];
            FC.For(num, (i) =>
            {
                var type = Srand.Next(0, BattleCard.AllTypeOfBattleCards.Length);
                cards[i] = BattleCard.AllTypeOfBattleCards[type];
            });

            return cards;
        }

        // 推动战斗基本流程
        public Battle BattleStatusTransfer(params int[] players)
        {
            var playerSeq = new List<int>();
            playerSeq.AddRange(players);

            // 所有玩家完成战斗准备，自动开始新回合
            AfterPlayerPrepared += (int player) => 
            {
                if (AllPrepared)
                    StartNextRound(playerSeq[0]);
            };

            AfterActionDone += (int player) =>
            {
                // 回合交替
                Debug.Assert(player == playerSeq[0]);
                playerSeq.RemoveAt(0);
                if (playerSeq.Count == 0)
                    playerSeq.AddRange(players);

                StartNextRound(playerSeq[0]);

                // 回合结束时检查战斗结束条件
                TryBattleEnd();
            };

            return this;
        }

        // 回合开始时重置护盾，重置行动标记，填充卡片等
        public Battle ResetDefenceAtRoundStart()
        {
            BeforeStartNextRound += (int player) =>
            {
                Map.ForeachWarriors((i, j, warrior) =>
                {
                    if (warrior.Owner != player)
                        return;

                    warrior.Shield = 0; // 重置所有护甲
                    warrior.Moved = false; // 重置行动标记
                    warrior.ActionDone = false; // 重置行动标记
                });
            };

            return this;
        }

        // 填充卡片
        public Battle AutoGenBattleCards(Func<int, int> getNumCardsGen = null, Action<int, BattleCard[]> onCardsGenerated = null)
        {
            BeforeStartNextRound += (int player) =>
            {
                // 填充卡片
                var num = getNumCardsGen == null ? 0 : getNumCardsGen(player);
                if (num > 0)
                    onCardsGenerated(player, GenNextCards(num));
            };

            return this;
        }

        // 结束战斗
        public event Action<int> OnBattleEnded = null; // 战斗结束通知
        public virtual void TryBattleEnd()
        {
            var r = CheckEndCondition();
            if (r != 0)
                OnBattleEnded.SC(r);
        }
    }
}
