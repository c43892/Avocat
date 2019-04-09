using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    // 将复杂战斗逻辑拆分到此
    public partial class Battle
    {
        protected void BuildLogic()
        {
            BuildBattleStatusTransfer();
            ResetDefenceAtRoundStart();
        }

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
        void BuildBattleStatusTransfer()
        {
            // 所有玩家完成战斗准备，自动开始新回合
            AfterPlayerPrepared += (int player) => 
            {
                if (AllPrepared)
                    StartNextRound(0);
            };

            // 回合结束时检查战斗结束条件
            AfterActionDone += (int player) => TryBattleEnd();
        }

        // 回合开始时重置护盾，重置行动标记
        void ResetDefenceAtRoundStart()
        {
            BeforeStartNextRound += (int player) =>
            {
                ForeachWarriors((i, j, warrior) =>
                {
                    if (warrior.Owner != player)
                        return;

                    warrior.Shield = 0; // 重置所有护甲
                    warrior.Moved = false; // 重置行动标记
                    warrior.ActionDone = false; // 重置行动标记
                });
            };
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
