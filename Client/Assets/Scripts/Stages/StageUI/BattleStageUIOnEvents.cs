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
                    BattleStageUI.ActioDoneBtn.gameObject.SetActive(true);
                    BattleStageUI.gameObject.SetActive(true);
                    BattleStageUI.CardArea.RefreshEnergy(bt.Energy, bt.MaxEnergy);
                  //  BattleStageUI.SkillButtonUI.UpdateSkillState(bt.Energy);
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

        // 自动排序卡牌时刷新UI
        bt.OnSortBattleCard += (warrior,availableCards) =>
        {
            aniPlayer.Op(() =>
            {
                BattleStageUI.CardArea.RefreshCardsAvailable(availableCards);
            });
        };

        bt.OnAddEN += (den) => aniPlayer.Op(() => BattleStageUI.CardArea.RefreshEnergy(bt.Energy, bt.MaxEnergy));
        bt.OnAddEN += (den) => aniPlayer.Op(() => BattleStageUI.SkillButtonUI.UpdateSkillState(bt.Energy, (BattleStageUI.BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior));
        bt.OnAddCardDissambleValue += (dv) => aniPlayer.Op(() => BattleStageUI.RefreshItemUsage(bt.CardUsage));

        InBattleOps.OnCurrentWarriorChanged += () => aniPlayer.Op(() => BattleStageUI.CardArea.RefreshEnergy(bt.Energy, bt.MaxEnergy));
        InBattleOps.OnCurrentWarriorChanged += () => aniPlayer.Op(() => BattleStageUI.RefreshItemUsage(bt.CardUsage));

        BattleStageUI.BattleStage.Battle.OnPlayerPrepared += (int num) =>
        {
            var CardAreaTransform = BattleStageUI.CardArea.transform;
            CardAreaTransform.parent.gameObject.SetActive(true);
        };

        BattleStageUI.BattleStage.Battle.OnPlayerPrepared += (int num) =>
        {
            BattleStageUI.BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                avatar.IsShowClickFrame = false;
            });
        };

        BattleStageUI.BattleStage.Battle.OnPlayerPrepared += (int num) =>
        {
            BattleStageUI.BattleStage.ForeachMapTile((x, y, mapTile) =>
            {
                mapTile.transform.Find("RespawnPlace").gameObject.SetActive(false);
            });
        };
    }
}
