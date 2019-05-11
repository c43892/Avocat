using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗卡牌区域
/// </summary>
public class BattleCardArea : MonoBehaviour
{
    // 可用的卡组区域
    public BattleCardUI[] CardsAvailableGroup;

    // 暂存卡牌区域
    public BattleCardUI[] CardsStashGroup;

    // 能量条
    public Image EnergyBar;

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

    // 刷新能量值显示
    public void RefreshEnergy(int energy, int maxEnerge)
    {
        EnergyBar.rectTransform.anchorMax = new Vector2(energy / (float)maxEnerge, 1);
    }

    // 刷新卡牌选中状态
    public void SetCardSels(int cnt)
    {
        FC.ForEach(CardsAvailableGroup, (i, c) => c.SelectedInPath = i < cnt);
    }
}
