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

    // 卡牌区域
    public BattleCardArea CardArea;
    public SkillButtonUI SkillButtonUI;
    public CharacterInfoUI CharacterInfoUI;

    public ObstacleUI obstacleUI;

    public MapAniPlayer AniPlayer { get => BattleStage.GetComponent<MapAniPlayer>(); }

    // 能量槽显示
    public GameObject Enerygy;

    // 建设值显示
    public GameObject ItemUsage;

    // 回合结束按钮
    public GameObject ActioDoneBtn;

    BattleRoomClient Room { get { return BattleStage.Room; } }

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
            BattleStage.StartSkillStage(true);
            BattleStage.StartSkill(cx, cy, (IWithRange)skill, (selX, selY) =>
            {               
                if (BattleStage.InBattleOps.CurrentSelWarrior.MovingPath.Count > 0)
                {
                    BattleStage.InBattleOps.ClearPath();
                    Room.DoMoveOnPath(warrior);
                }
                Room.FireActiveSkillAt(skill, selX, selY);
            });
        }
        else {
            if (BattleStage.InBattleOps.CurrentSelWarrior.MovingPath.Count > 0)
            {
                BattleStage.SkillOps.RemoveShowRange();
                BattleStage.InBattleOps.RemoveShowAttackRange();
                BattleStage.InBattleOps.ClearPath();
                Room.DoMoveOnPath(warrior);
            }
            Room.FireActiveSkill(skill);
        }       
    }

    // 进入地图改造模式
    bool inUsingItemMode = false;
    public void OnChange2UsingItem()
    {
        inUsingItemMode = !inUsingItemMode;
        BattleStage.StartUseItem(inUsingItemMode);
        ItemUsage.GetComponent<Image>().color = inUsingItemMode ? Color.red : Color.yellow;

        // FC.ForEach(CardsAvailableGroup, (i, c) => c.gameObject.SetActive(!inUsingItemMode));
        // FC.ForEach(CardsStashGroup, (i, c) => c.gameObject.SetActive(!inUsingItemMode));
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
