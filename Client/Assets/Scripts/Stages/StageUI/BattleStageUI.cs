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

    // 可用的卡组区域
    public BattleCardUI[] CardsAvailableGroup;

    // 暂存卡牌区域
    public BattleCardUI[] CardsStashGroup;

    // 能量槽显示
    public GameObject Enerygy;

    BattleRoomClient Room { get { return BattleStage.Room; } }

    // 刷新战斗卡牌区域
    public void RefreshCardsAvailable(List<BattleCard> cards = null)
    {
        FC.ForEach(CardsAvailableGroup, (i, c) => c.Card = i < cards.Count ? cards[i] : null);
    }

    // 刷新暂存卡牌区域
    public void RefreshCardsStarshed(List<BattleCard> cards = null)
    {
        FC.ForEach(CardsStashGroup, (i, c) => c.Card = i < cards.Count ? cards[i] : null);
    }

    // 刷新能量槽
    public void RefreshEnergy(int energy)
    {
        Enerygy.GetComponent<Image>().color = energy == 100 ? Color.green : Color.white;
        Enerygy.GetComponentInChildren<Text>().text = energy.ToString();
    }

    // 结束当前回合
    public void OnActionDone()
    {
        Room.ActionDone();
    }
}
