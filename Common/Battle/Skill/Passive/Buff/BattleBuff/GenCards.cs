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
    public class GenCards : BattleBuff
    {
        public override string ID { get => "GenCards"; }
        readonly Action<int, BattleCard[]> CardsGeneratedCallback = null;

        public GenCards(Battle bt, Action<int, BattleCard[]> onCardsGenerated = null)
            : base(bt)
        {
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

        readonly int Team = 1; // 玩家 only
        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += (int team) =>
            {
                if (team != Team) 
                    return;

                // 填充卡片

                // 两张随机
                var twoRandomCards = GenNextCards(2);
                // 每个英雄一张
                var cardsByHeros = new List<BattleCard>();
                Battle.Map.ForeachObjs<Hero>((x, y, hero) =>
                {
                    if (hero.Team != Team || hero.CardType == null)
                        return;

                    cardsByHeros.Add(BattleCard.Create(hero.CardType));
                });

                cardsByHeros.InsertRange(0, twoRandomCards);
                CardsGeneratedCallback(Team, cardsByHeros.ToArray());
            };

            base.OnAttached();
        }
    }
}
