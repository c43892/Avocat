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
    public bool enterSkillStage;

    // 可用的卡组区域
    public BattleCardUI[] CardsAvailableGroup;

    // 暂存卡牌区域
    public BattleCardUI[] CardsStashGroup;

    // 能量槽显示
    public GameObject Enerygy;

    // 建设值显示
    public GameObject ItemUsage;

    // 回合结束按钮
    public GameObject ActioDoneBtn;

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

    public int selX;
    public int selY;
    // 释放主动技能
    public void OnFireActiveSkill()
    {
        enterSkillStage = true;
        var skill = (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior?.GetDefaultActiveSkill();
        Debug.Assert(skill != null, "there is no default active skill");
        Warrior warrior = BattleStage.InBattleOps.CurrentSelWarrior;
        if (skill.ActiveSkillType == "fireAt")
        {
            // confirm the position and fire at it
            var cx = 0;
            var cy = 0;
            warrior.GetPosInMap(out int x, out int y);
            List<int> movingPath = (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior?.MovingPath;
            if (movingPath != null && movingPath.Count >= 2)
            {
                cx = movingPath[movingPath.Count - 2];
                cy = movingPath[movingPath.Count - 1];
            }
            else
            {
                cx = x;
                cy = y;
            }
            BattleStage.StartSkillStage(enterSkillStage);
            BattleStage.SkillOps.ShowSkillAttackRange(cx, cy, (IWithRange)skill, null, null);
            StartCoroutine(Example(skill,warrior));
            //if (enterSkillStage)
            //{
            //    BattleStage.StartSkill(cx, cy, (IWithRange)skill, (selx, sely) =>
            //      {
            //          Room.FireActiveSkillAt(skill, selx, sely);
            //      },()=> {
            //          Room.DoMoveOnPath(warrior);
            //      });
            //}

        }
        else
            Room.FireActiveSkill(skill);
    }

    public IEnumerator Example(ActiveSkill skill, Warrior warrior)
    {
        yield return new WaitUntil(() => BattleStage.SkillOps.choosePos == true);
        if (BattleStage.InBattleOps.CurrentSelWarrior.MovingPath.Count > 0)
        {
            BattleStage.InBattleOps.ClearSelTiles();
            Room.DoMoveOnPath(warrior);
        }
        Room.FireActiveSkillAt(skill, BattleStage.SkillOps.posX, BattleStage.SkillOps.posY);
    }

    // 进入地图改造模式
    bool inUsingItemMode = false;
    public void OnChange2UsingItem()
    {
        inUsingItemMode = !inUsingItemMode;
        BattleStage.StartUseItem(inUsingItemMode);
        ItemUsage.GetComponent<Image>().color = inUsingItemMode ? Color.red : Color.yellow;

        FC.ForEach(CardsAvailableGroup, (i, c) => c.gameObject.SetActive(!inUsingItemMode));
        FC.ForEach(CardsStashGroup, (i, c) => c.gameObject.SetActive(!inUsingItemMode));
        Enerygy.gameObject.SetActive(!inUsingItemMode);
        ActioDoneBtn.gameObject.SetActive(!inUsingItemMode); 
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
