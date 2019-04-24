using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;
using UnityEngine.EventSystems;

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

    // 建设值显示
    public GameObject ItemUsage;

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
        Enerygy.GetComponent<Image>().color = energy == 100 ? Color.yellow : Color.white;
        Enerygy.GetComponentInChildren<Text>().text = energy.ToString();

        var skill = (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior?.GetDefaultActiveSkill();
        if (skill != null && energy >= skill.EnergyCost)
        {
            Enerygy.GetComponent<Button>().interactable = true;
            Enerygy.GetComponent<Image>().color = Color.green;
        }
        else
        {
            Enerygy.GetComponent<Button>().interactable = false;
            Enerygy.GetComponent<Image>().color = Color.red;
        }
    }

    // 刷新建设值
    public void RefreshItemUsage(int v)
    {
        ItemUsage.GetComponent<Image>().color = inUsingItemMode ? Color.red : (v == 100 ? Color.yellow : Color.white);
        ItemUsage.GetComponentInChildren<Text>().text = v.ToString();
        ItemUsage.GetComponent<Button>().interactable = inUsingItemMode || v == 100;
    }

    // 结束当前回合
    public void OnActionDone()
    {
        Room.ActionDone();
    }

    // 释放主动技能
    public void OnFireActiveSkill()
    {
        var skill = (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior?.GetDefaultActiveSkill();
        Debug.Assert(skill != null, "there is no default active skill");

        if (skill.ActiveSkillType == "fireAt")
        {
            // confirm the position and fire at it
            Room.FireActiveSkillAt(skill, 0, 0);
        }
        else
            Room.FireActiveSkill(skill);
    }

    // 进入地图改造模式
    bool inUsingItemMode = false;
    public void OnChange2UsingItem()
    {
        inUsingItemMode = !inUsingItemMode;
        BattleStage.StartUseItem(inUsingItemMode);
        ItemUsage.GetComponent<Image>().color = inUsingItemMode ? Color.red : Color.yellow;
    }

    // 检查当前鼠标是否点击在 ui 上
    public static bool CheckGuiRaycastObjects()
    {
        var raycaster = Camera.main.transform.root.GetComponentInChildren<GraphicRaycaster>();
        var evnSystem = Camera.main.transform.root.GetComponentInChildren<EventSystem>();
        var eventData = new PointerEventData(evnSystem)
        {
            pressPosition = Input.mousePosition,
            position = Input.mousePosition
        };

        var list = new List<RaycastResult>();
        raycaster.Raycast(eventData, list);
        return list.Count > 0;
    }
}
