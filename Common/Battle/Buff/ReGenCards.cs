using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 每回合重新生成战斗卡牌
    /// </summary>
    public class ReGenCards : Buff
    {
        Func<int, int> NumCardsGen = null;
        Action<int, BattleCard[]> CardsGeneratedCallback = null;

        public ReGenCards(Func<int, int> getNumCardsGen = null, Action<int, BattleCard[]> onCardsGenerated = null)
        {
            NumCardsGen = getNumCardsGen;
            CardsGeneratedCallback = onCardsGenerated;
        }

        // 生成随机战斗卡牌
        BattleCard[] GenNextCards(int num)
        {
            var cards = new BattleCard[num];
            FC.For(num, (i) =>
            {
                var type = Battle.Srand.Next(0, BattleCard.BattleCardTypesNum);
                cards[i] = BattleCard.Create(type);
            });

            return cards;
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound.Add((int player) =>
            {
                // 填充卡片
                var num = NumCardsGen == null ? 0 : NumCardsGen(player);
                if (num > 0)
                    CardsGeneratedCallback(player, GenNextCards(num));
            });
        }
    }
}
