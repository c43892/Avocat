using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 每回合分解未使用的战斗卡牌
    /// </summary>
    public class DisassembleCards : BattleBuff
    {
        public override string ID { get => "DisassembleCards"; }
        public BattlePVE BT { get => Battle as BattlePVE; }
        public List<BattleCard> AvailableCards { get; private set; }

        readonly Action<int, BattleCard[]> OnCardsDisassembledDone;

        public DisassembleCards(Battle bt, List<BattleCard> cardList, Action<int, BattleCard[]> onCardsDisassembledDone)
            : base(bt)
        {
            AvailableCards = cardList;
            OnCardsDisassembledDone = onCardsDisassembledDone;
        }

        void DissambleCards(int team)
        {
            // 每张卡牌增加一定建设值
            var cards = AvailableCards.ToArray();
            BT.AddCardDissambleValue(cards.Length * 20);

            AvailableCards.Clear();
            OnCardsDisassembledDone?.Invoke(team, cards);
        }

        public override void OnAttached()
        {
            Battle.BeforeActionDone += DissambleCards;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            throw new Exception("not implemented yet");
        }
    }
}
