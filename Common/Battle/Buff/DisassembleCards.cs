using System;
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
        public List<BattleCard> AvailableCards { get; private set; }
        int Player { get; set; }
        readonly Action<int, BattleCard[]> OnCardsDisassembled;

        public DisassembleCards(int player, List<BattleCard> cardList, Action<int, BattleCard[]> onCardsDisassembled)
        {
            AvailableCards = cardList;
            Player = player;
            OnCardsDisassembled = onCardsDisassembled;
        }

        public override void OnAttached()
        {
            Battle.BeforeActionDone.Add((int player) =>
            {
                if (Player != player)
                    return;

                var cards = AvailableCards.ToArray();
                AvailableCards.Clear();
                OnCardsDisassembled?.Invoke(player, cards);
            });
        }
    }
}
