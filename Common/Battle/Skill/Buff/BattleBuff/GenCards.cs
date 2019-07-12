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
        public BattlePVE BT { get => Battle as BattlePVE; }

        public GenCards(Battle bt) : base(bt) { }

        readonly int Team = 1; // 玩家 only
        public override void OnAttached()
        {
            Battle.BeforeStartNextRound1 += (int team) =>
            {
                if (team != Team) 
                    return;

                // 填充卡片

                // 两张随机
                var twoRandomCards = Battle.GenNextCards(2);

                // 每个英雄一张
                var cardsByHeros = new List<BattleCard>();
                Battle.Map.ForeachObjs<Hero>((x, y, hero) =>
                {
                    if (hero.Team != Team || hero.CardType == null)
                        return;

                    cardsByHeros.Add(BattleCard.Create(hero.CardType));
                });

                cardsByHeros.AddRange(twoRandomCards);
                BT.AddCards(cardsByHeros);
            };

            base.OnAttached();
        }

        public override void OnDetached()
        {
            throw new Exception("not implemented yet");
        }
    }
}
