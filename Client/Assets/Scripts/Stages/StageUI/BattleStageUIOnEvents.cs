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
        var aniPlayer = BattleStageUI.AniPlayer;

        bt.OnPlayerPrepared += (int player) =>
        {
            if (room.Battle.AllPrepared)
            {
                aniPlayer.Op(() =>
                {
                    BattleStageUI.gameObject.SetActive(true);
                    BattleStageUI.RefreshEnergy(bt.Energy);
                    BattleStageUI.RefreshItemUsage(bt.CardUsage);
                });
            }
        };

        bt.OnBattleEnded += (winner) =>
        {
            aniPlayer.Op(() => BattleStageUI.gameObject.SetActive(false));
        };

        bt.OnActionDone += (player) =>
        {
            if (player != room.PlayerMe)
                return;

            aniPlayer.Op(() => BattleStageUI.CardArea.RefreshCardsAvailable(bt.AvailableCards));
        };

        bt.OnAddBattleCard += (card) =>
        {
            aniPlayer.Op(() => BattleStageUI.CardArea.RefreshCardsAvailable(bt.AvailableCards));
        };

        bt.OnNextRoundStarted += (player) =>
        {
            if (player != room.PlayerMe)
                return;

            aniPlayer.Op(() =>
            {
                var availableCards = new List<BattleCard>();
                availableCards.AddRange(bt.AvailableCards);
                BattleStageUI.CardArea.RefreshCardsAvailable(availableCards);
            });
        };

        bt.OnCardsConsumed += (warrior, cards) =>
        {
            if (warrior.Team != room.PlayerMe)
                return;

            aniPlayer.Op(() =>
            {
                var availableCards = new List<BattleCard>();
                availableCards.AddRange(bt.AvailableCards);
                BattleStageUI.CardArea.RefreshCardsAvailable(availableCards);
            });
        };

        bt.OnBattleCardsExchange += (int g1, int n1, int g2, int n2) =>
        {
            aniPlayer.Op(() =>
            {
                BattleStageUI.CardArea.RefreshCardsAvailable(bt.AvailableCards);
                BattleStageUI.CardArea.RefreshCardsStarshed(bt.StashedCards);
            });
        };

        bt.OnAddEN += (den) => aniPlayer.Op(() => BattleStageUI.RefreshEnergy(bt.Energy));
        bt.OnAddCardDissambleValue += (dv) => aniPlayer.Op(() => BattleStageUI.RefreshItemUsage(bt.CardUsage));

        InBattleOps.OnCurrentWarriorChanged += () => aniPlayer.Op(() => BattleStageUI.RefreshEnergy(bt.Energy));
    }
}
