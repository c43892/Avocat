using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

/// <summary>
/// 准备阶段操作：移动英雄位置
/// </summary>
public class InBattleOps : StageOpsLayer
{
    public InBattleOps(BattleStage bs)
        : base(bs)
    {
    }

    // selectingWarrior - 等待选择行动对象, selectingAttackTarget - 等待选择攻击目标，selectingPath - 选择行动路径
    string status = "selectingWarrior";
    Warrior CurrentSelWarrior
    {
        get
        {
            return curSelWarrior;
        }
        set
        {
            if (curSelWarrior == value)
                return;

            if (curSelWarrior != null && curSelWarrior.GetPosInMap(out int x, out int y))
                BattleStage.Avatars[x, y].Selected = false;

            curSelWarrior = value;
            if (curSelWarrior != null && curSelWarrior.GetPosInMap(out x, out y))
                BattleStage.Avatars[x, y].Selected = true;

            FC.ForEach(pathInSel, (i, tile) => tile.Selected = false);
            pathInSel.Clear();
        }
    } Warrior curSelWarrior;

    public override void OnClicked(float x, float y)
    {
        // 获取当前点击目标
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        var warrior = avatar == null ? null : avatar.Warrior;

        switch (status)
        {
            case "selectingWarrior":
                if (warrior != null && !warrior.IsOpponent)
                {
                    // 选中己方角色，等待攻击指令
                    CurrentSelWarrior = warrior;
                    status = "selectingAttackTarget";
                }
                break;
            case "selectingAttackTarget":
                if (warrior == null)
                {
                    // 点空地恢复初始状态
                    CurrentSelWarrior = null;
                    status = "selectingWarrior";
                }
                else if (!warrior.IsOpponent)
                {
                    // 点己方角色，切换操作对象
                    CurrentSelWarrior = warrior;
                }
                else if (CurrentSelWarrior != null)
                {
                    // 点对方角色，指定攻击指令
                    DoAttack(CurrentSelWarrior, warrior);
                    CurrentSelWarrior = null;
                    status = "selectingWarrior";
                }
                break;
        }
    }

    // 拖拽连续选择行动路径
    List<MapTile> pathInSel = new List<MapTile>();
    public override void OnStartDragging(float x, float y)
    {
        // 获取当前点击目标
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        var warrior = avatar == null ? null : avatar.Warrior;
        if (warrior == null)
        {
            // 从空地拖拽和点空地一样的效果
            CurrentSelWarrior = null;
            status = "selectingWarrior";
            return;
        }

        pathInSel.Clear();
        CurrentSelWarrior = warrior;
        var tile = BattleStage.Tiles[(int)x, (int)y];
        tile.Selected = true;
        pathInSel.Add(tile);
        status = "selectingPath";
    }

    public override void OnDragging(float fx, float fy, float cx, float cy)
    {
        if (status != "selectingPath")
            return;

        var tile = BattleStage.Tiles[(int)cx, (int)cy];
        var tailTile = pathInSel[pathInSel.Count - 1];
        if (tile == tailTile)
            return;

        var secondTailTile = pathInSel.Count < 2 ? null : pathInSel[pathInSel.Count - 2];
        if (tile == secondTailTile)
        {
            // 回退指向尾部第二块，则从路径中退掉尾部第一块
            pathInSel.RemoveAt(pathInSel.Count - 1);
            tailTile.Selected = false;
        }
        else
        {
            // 拖拽到相邻块则加入路径
            var dist = MU.ManhattanDist(tailTile.X, tailTile.Y, tile.X, tile.Y);
            if (dist == 1)
            {
                pathInSel.Add(tile);
                tile.Selected = true;
            }
        }
    }

    public override void OnEndDragging(float fx, float fy, float cx, float cy)
    {
        if (status != "selectingPath")
            return;

        status = "selectingAttackTarget";
    }

    // 执行攻击指令
    void DoAttack(Warrior attacker, Warrior target)
    {
        Debug.Log("do attack");
    }
}
