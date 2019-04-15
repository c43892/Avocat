using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Swift;
using Avocat;

/// <summary>
/// 战斗卡牌 UI
/// </summary>
public class BattleCardUI : MonoBehaviour, IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    // 拖动中的卡牌
    static BattleCardUI CardInDragging { get; set; }

    public int Group = 0;
    public int IndexInGroup = 0;

    // 处理卡牌拖动交换事件
    public static event Action<int, int, int, int> OnCardExchanged = null;

    Vector2 originalPos = Vector2.zero;
    public BattleCard Card
    {
        get
        {
            return card;
        }
        set
        {
            card = value;
            if (card == null)
                GetComponentInChildren<Text>().text = null;
            else
                GetComponentInChildren<Text>().text = card.Name;
        }
    } BattleCard card;

    Vector2 draggingStartPos = Vector2.zero;
        public void OnBeginDrag(PointerEventData eventData)
    {
        CardInDragging = this;
        draggingStartPos = eventData.position;
        originalPos = transform.GetChild(0).localPosition;
        CardInDragging.transform.SetSiblingIndex(-1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.GetChild(0).localPosition = originalPos + (eventData.position - draggingStartPos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (CardInDragging.card != card)
            OnCardExchanged.SC(CardInDragging.Group, CardInDragging.IndexInGroup, Group, IndexInGroup);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.GetChild(0).localPosition = originalPos;
        CardInDragging = null;
    }
}
