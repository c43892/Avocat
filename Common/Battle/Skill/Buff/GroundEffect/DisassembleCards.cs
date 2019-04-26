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
    public class DisassembleCards : Buff
    {
        public BattlePVE BattlePVE { get => this.Battle as BattlePVE; }
        public List<BattleCard> AvailableCards { get; private set; }
        int Player { get; set; }
        readonly Action<int, BattleCard[]> OnCardsDisassembledDone;

        public DisassembleCards(int player, List<BattleCard> cardList, Action<int, BattleCard[]> onCardsDisassembledDone)
        {
            AvailableCards = cardList;
            Player = player;
            OnCardsDisassembledDone = onCardsDisassembledDone;
        }

        IEnumerator DissambleCards(int player)
        {
            if (Player != player)
                yield break;

            // 每张卡牌增加一定建设值
            var cards = AvailableCards.ToArray();
            yield return BattlePVE.AddCardDissambleValue(cards.Length * Calculation.ItemUsagePerCard);

            AvailableCards.Clear();
            OnCardsDisassembledDone?.Invoke(player, cards);
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeActionDone.Add(DissambleCards);

            yield return base.OnAttached();
        }
    }
}
