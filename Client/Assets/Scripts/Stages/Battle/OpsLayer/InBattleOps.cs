using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

/// <summary>
/// 准备阶段操作：移动英雄位置
/// </summary>
public class InBattleOps : StageOpsLayer
{
    public InBattleOps(BattleStage bs)
        : base(bs)
    {
        BattleCardUI.OnCardExchanged += OnCardExchanged;
    }

    // 交换卡牌
    private void OnCardExchanged(int gf, int nf, int gt, int nt)
    {
        if (gf == gt && nf == nt)
            return;

        Room.DoExchangeBattleCards(gf, nf, gt, nt);
    }

    // selectingWarrior - 等待选择行动对象, selectingAttackTarget - 等待选择攻击目标，selectingPath - 选择行动路径
    string status = "selectingWarrior";
    public static Action OnCurrentWarriorChanged = null;
    public Warrior CurrentSelWarrior
    {
        get
        {
            return curSelWarrior;
        }
        private set
        {
            if (curSelWarrior == value || (value != null && value.ActionDone))
                return;

            if (curSelWarrior != null && curSelWarrior.Map != null)
            {
                curSelWarrior.GetPosInMap(out int x, out int y);
                BattleStage.Avatars[x, y].Color = MapAvatar.ColorDefault;
            }

            curSelWarrior = value;
            if (curSelWarrior != null)
            {
                curSelWarrior.GetPosInMap(out int x, out int y);
                BattleStage.Avatars[x, y].Color = MapAvatar.ColorSelected;
                curSelWarrior.MovingPath.Clear();
            }

            ClearSelTiles();

            OnCurrentWarriorChanged?.Invoke();
        }
    } Warrior curSelWarrior;

    List<MapTile> pathATKRange = new List<MapTile>();
    // 显示攻击范围
    public void ShowAttackRange(float x, float y, Warrior worrior)
    {
        Debug.Assert(pathATKRange.Count == 0, "path range is not empty now.");
        // MU.ManhattanDist(x, y, tx, ty) <= AttackRange
        FC.For(worrior.AttackRange.Length,(i) => {
            int minX = (x - worrior.AttackRange[i]) < 0 ? 0 : (int)(x - worrior.AttackRange[i]);
            int maxX = (x + worrior.AttackRange[i]) >= BattleStage.Map.Width ? BattleStage.Map.Width : (int)(x + worrior.AttackRange[i] + 1);
            int minY = (y - worrior.AttackRange[i]) < 0 ? 0 : (int)(y - worrior.AttackRange[i]);
            int maxY = (y + worrior.AttackRange[i]) >= BattleStage.Map.Height ? BattleStage.Map.Height : (int)(y + worrior.AttackRange[i] + 1);
            FC.For2(minX, maxX, minY, maxY, (tx, ty) =>
            {
                if (MU.ManhattanDist((int)x, (int)y, tx, ty) == worrior.AttackRange[i])
                {
                    var tile = BattleStage.CreateMapTile(tx, ty);
                    pathATKRange.Add(tile);
                }
            });
        });

        FC.For(0, pathATKRange.Count, i =>
        {
            pathATKRange[i].GetComponent<SpriteRenderer>().color = Color.red;
        });
    }

    // 清除路劲显示
    public void ClearSelTiles()
    {
        FC.ForEach(pathInSel, (i, tile) =>
        {
            tile.Color = MapTile.ColorDefault;
            tile.Card = null;
        });
        pathInSel.Clear();
    }

    // 清除攻击范围显示
    public void RemoveShowAttackRange()
    {
        BattleStage.RemoveTile(pathATKRange);
    }

    public override void OnClicked(float x, float y)
    {
        // 获取当前点击目标
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        var warrior = avatar == null ? null : avatar.Warrior;

        switch (status)
        {
            case "selectingWarrior":
                if (warrior != null && warrior.Team == Room.PlayerMe)
                {
                    // 选中己方角色，等待攻击指令
                    CurrentSelWarrior = warrior;
                    if (CurrentSelWarrior != null) {
                        status = "selectingAttackTarget";
                        ShowAttackRange(x, y, CurrentSelWarrior);
                    }
                        
                }
                break;
            case "selectingAttackTarget":
                // 再次点击时清除攻击范围显示
                RemoveShowAttackRange();
                if (warrior == null || CurrentSelWarrior == null || CurrentSelWarrior.ActionDone)
                {
                    // 点空地

                    if (CurrentSelWarrior.MovingPath.Count >= 2
                        && CurrentSelWarrior.MovingPath[CurrentSelWarrior.MovingPath.Count - 2] == (int)x
                        && CurrentSelWarrior.MovingPath[CurrentSelWarrior.MovingPath.Count - 1] == (int)y)
                    {
                        // 如果是移动目的地，则直接移动
                        if (CurrentSelWarrior.MovingPath.Count > 0)
                            Room.DoMoveOnPath(CurrentSelWarrior);

                        CurrentSelWarrior = null;
                        status = "selectingWarrior";
                    }
                    else
                    {
                        // 否则恢复初始状态
                        CurrentSelWarrior = null;
                        status = "selectingWarrior";
                    }
                }
                else if (warrior.Team == Room.PlayerMe)
                {
                    // 点己方角色，切换操作对象
                    CurrentSelWarrior = warrior;
                    if (CurrentSelWarrior != null && !warrior.ActionDone)
                        ShowAttackRange(x, y, CurrentSelWarrior);
                }
                else if (CurrentSelWarrior != null)
                {
                    // 点对方角色，指定攻击指令
                    if (CurrentSelWarrior.MovingPath.Count > 0)
                        Room.DoMoveOnPath(CurrentSelWarrior);

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
        if (warrior == null || warrior.Moved || warrior.Team != Room.PlayerMe)
        {
            // 从空地拖拽和点空地一样的效果
            CurrentSelWarrior = null;
            status = "selectingWarrior";
            return;
        }

        CurrentSelWarrior = warrior;
        var tile = BattleStage.Tiles[(int)x, (int)y];
        pathInSel.Clear();
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
            tailTile.Color = MapTile.ColorDefault;
            tailTile.Card = null;
            if (pathInSel.Count > 1) // 头节点不变色显示
            {
                pathInSel[pathInSel.Count - 1].Color = MapTile.ColorSelectedHead;
                pathInSel[pathInSel.Count - 1].SetDir(0, 0);
            }
        }
        else if (!pathInSel.Contains(tile)) // 指向队列中的中间某一块，则忽略该块
        {
            // 已经达到移动距离上限，也不再增加。起始点不应算在距离限制中，所以 +1
            if (pathInSel.Count >= CurrentSelWarrior.MoveRange + 1)
                return;

            // 拖拽到相邻块则加入路径
            var dist = MU.ManhattanDist(tailTile.X, tailTile.Y, tile.X, tile.Y);
            if (dist == 1 && !Room.Battle.Map.BlockedAt(tile.X, tile.Y))
            {
                if (pathInSel.Count > 1) // 头节点不变色显示
                {
                    var cd = pathInSel[pathInSel.Count - 1];
                    cd.Color = MapTile.ColorSelected;
                    cd.SetDir(tile.X - tailTile.X, tile.Y - tailTile.Y);
                    cd.Card = (Room.Battle as BattlePVE).AvailableCards[pathInSel.Count - 2]; // 第一个路径节点并不对应战斗卡牌
                }

                tile.Color = MapTile.ColorSelectedHead;
                tile.Card = (Room.Battle as BattlePVE).AvailableCards[pathInSel.Count - 1];
                tile.SetDir(0, 0);
                pathInSel.Add(tile);
            }
        }
    }

    public override void OnEndDragging(float fx, float fy, float cx, float cy)
    {
        if (status != "selectingPath")
            return;

        CurrentSelWarrior.MovingPath.Clear();
        FC.For(1, pathInSel.Count, (i) => // 第一个节点是目前所在位置，应从路径中剔除
        {
            var tile = pathInSel[i];
            CurrentSelWarrior.MovingPath.Add(tile.X);
            CurrentSelWarrior.MovingPath.Add(tile.Y);
        });

        status = "selectingAttackTarget";
        // 显示移动结束时攻击范围
        int lastX = pathInSel[pathInSel.Count - 1].X;
        int lastY = pathInSel[pathInSel.Count - 1].Y;
        RemoveShowAttackRange();
        ShowAttackRange((float)lastX, (float)lastY, CurrentSelWarrior);
    }

    // 执行攻击指令
    void DoAttack(Warrior attacker, Warrior target)
    {
        Room.DoAttack(attacker, target);
    }
}
