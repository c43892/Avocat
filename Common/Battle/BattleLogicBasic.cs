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
                    StartNextRound();
            };

            // 回合结束时检查战斗结束条件
            AfterActionDone += (int player) => TryBattleEnd();
        }

        void ResetDefenceAtRoundStart()
        {
            BeforeStartNextRound += () =>
            {
                // 重置所有护甲
                ForeachWarriors((i, j, warrior) => warrior.Shield = 0);
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
