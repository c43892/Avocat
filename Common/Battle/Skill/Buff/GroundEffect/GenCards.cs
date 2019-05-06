using System;
using System.Collections;
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
    public class GenCards : Buff
    {
        readonly Action<int, BattleCard[]> CardsGeneratedCallback = null;
        int Player { get; set; }

        public GenCards(int player, Action<int, BattleCard[]> onCardsGenerated = null)
        {
            Player = player;
            CardsGeneratedCallback = onCardsGenerated;
        }

        // 生成随机战斗卡牌
        BattleCard[] GenNextCards(int num)
        {
            var cards = new BattleCard[num];
            FC.For(num, (i) =>
            {
                var type = Battle.Srand.Next(0, BattleCard.BattleCardTypesNum);
                cards[i] = BattleCard.Create(BattleCard.CardTypes[type]);
            });

            return cards;
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += (int player) =>
            {
                if (Player != player)
                    return;

                // 填充卡片

                // 两张随机
                var twoRandomCards = GenNextCards(2);
                // 每个英雄一张
                var cardsByHeros = new List<BattleCard>();
                Battle.Map.ForeachWarriors(Player, (x, y, warrior) =>
                {
                    if (!(warrior is Hero))
                        return;

                    cardsByHeros.Add(BattleCard.Create((warrior as Hero).CardType));
                });

                cardsByHeros.InsertRange(0, twoRandomCards);
                CardsGeneratedCallback(player, cardsByHeros.ToArray());
            };

            base.OnAttached();
        }
    }
}
