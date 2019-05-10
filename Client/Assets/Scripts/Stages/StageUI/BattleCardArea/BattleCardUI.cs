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
public class BattleCardUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    // 拖动中的卡牌
    static BattleCardUI CardInDragging { get; set; }

    public GameObject BgSel; // 选中状态背景框
    public Image CardType; // 卡牌类型

    public int Group { get; set; } // 可用卡牌区域是 0，暂存区是 1
    public int IndexInGroup { get; set; } // 在所属卡牌组中的顺序索引值

    // 是否在已选择的路径中
    public bool SelectedInPath { get; set; }

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
            if (card != null)
            {
                BgSel.SetActive(SelectedInPath);
                CardType.gameObject.SetActive(true);
                CardType.sprite = Resources.Load<Sprite>("UI/BattleCardArea/" + card.Name);
            }
            else
            {
                BgSel.SetActive(false);
                CardType.gameObject.SetActive(false);
            }
        }
    } BattleCard card;

    Vector2 draggingStartPos = Vector2.zero;
    public void OnPointerDown(PointerEventData eventData)
    {
        draggingStartPos = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardInDragging = this;
        originalPos = transform.GetChild(1).localPosition;
        CardInDragging.transform.SetSiblingIndex(-1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.GetChild(1).localPosition = originalPos + (eventData.position - draggingStartPos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (CardInDragging.card != card)
            OnCardExchanged.SC(CardInDragging.Group, CardInDragging.IndexInGroup, Group, IndexInGroup);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.GetChild(1).localPosition = originalPos;
        CardInDragging = null;
    }
}
