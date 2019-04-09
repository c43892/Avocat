using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

/// <summary>
/// 战斗场景 UI
/// </summary>
public class BattleStageUI : MonoBehaviour
{
    // 战斗场景
    public BattleStage BattleStage;

    // 战斗卡牌模板
    public BattleCardUI BattleCard;

    // 可用的卡组区域
    public Transform CardsAvailableGroup;

    BattleRoomClient Room { get { return BattleStage.Room; } }

    // 刷新一组新的战斗卡牌
    public void RefreshCardsAvailable(BattleCard[] cards)
    {
        ClearCardsAvailable();

        FC.ForEach(cards, (i, c) =>
        {
            var cd = Instantiate(BattleCard);
            cd.Card = c;
            cd.transform.SetParent(CardsAvailableGroup);
        });
    }

    // 清理当前战斗卡牌区域
    void ClearCardsAvailable()
    {
        while (CardsAvailableGroup.childCount > 0)
            DestroyImmediate(CardsAvailableGroup.GetChild(0));
    }

    // 结束当前回合
    public void OnActionDone()
    {
        Room.ActionDone();
    }
}
