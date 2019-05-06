using Avocat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class BattleStageUIOnEvents
{
    public static void SetupEventHandler(this BattleStageUI BattleStageUI, BattleRoomClient room)
    {
        var bt = BattleStageUI.BattleStage.Battle as BattlePVE;

        bt.OnPlayerPrepared += (int player) =>
        {
            if (room.Battle.AllPrepared)
            {
                BattleStageUI.gameObject.SetActive(true);
                BattleStageUI.RefreshEnergy(bt.Energy);
                BattleStageUI.RefreshItemUsage(bt.CardUsage);
            }
        };

        bt.OnBattleEnded += (winner) =>
        {
            BattleStageUI.gameObject.SetActive(false);
        };

        bt.OnActionDone += (player) =>
        {
            if (player != room.PlayerMe)
                return;

            BattleStageUI.RefreshCardsAvailable(bt.AvailableCards);
        };

        bt.OnAddBattleCard += (card) =>
        {
            BattleStageUI.RefreshCardsAvailable(bt.AvailableCards);
        };

        bt.OnNextRoundStarted += (player) =>
        {
            if (player != room.PlayerMe)
                return;

            var availableCards = new List<BattleCard>();
            availableCards.AddRange(bt.AvailableCards);
            BattleStageUI.RefreshCardsAvailable(availableCards);
        };

        bt.OnCardsConsumed += (warrior, cards) =>
        {
            if (warrior.Team != room.PlayerMe)
                return;

            var availableCards = new List<BattleCard>();
            availableCards.AddRange(bt.AvailableCards);
            BattleStageUI.RefreshCardsAvailable(availableCards);
        };

        bt.OnBattleCardsExchange += (int g1, int n1, int g2, int n2) =>
        {
            BattleStageUI.RefreshCardsAvailable(bt.AvailableCards);
            BattleStageUI.RefreshCardsStarshed(bt.StashedCards);
        };

        bt.OnAddEN += (den) => BattleStageUI.RefreshEnergy(bt.Energy);
        bt.OnAddCardDissambleValue += (dv) => BattleStageUI.RefreshItemUsage(bt.CardUsage);

        InBattleOps.OnCurrentWarriorChanged += () => BattleStageUI.RefreshEnergy(bt.Energy);
    }
}
