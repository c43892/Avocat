using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;
using UnityEngine;

/// <summary>
/// 战斗卡牌区域
/// </summary>
public class BattleCardArea : MonoBehaviour
{
    // 可用的卡组区域
    public BattleCardUI[] CardsAvailableGroup;

    // 暂存卡牌区域
    public BattleCardUI[] CardsStashGroup;

    private void Awake()
    {
        FC.ForEach(CardsAvailableGroup, (i, c) => { c.Group = 0; c.IndexInGroup = i; });
        FC.ForEach(CardsStashGroup, (i, c) => { c.Group = 1; c.IndexInGroup = i; });
    }

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
}
