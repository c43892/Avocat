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
    // 战斗卡牌模板
    public BattleCardUI BattleCard;

    // 可用的卡组区域
    public Transform CardsAvailableGroup;

    // 战斗卡牌
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

    void ClearCardsAvailable()
    {
        while (CardsAvailableGroup.childCount > 0)
            DestroyImmediate(CardsAvailableGroup.GetChild(0));
    }
}
