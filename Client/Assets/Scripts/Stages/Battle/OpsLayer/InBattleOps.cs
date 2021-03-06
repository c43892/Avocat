﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;
using UnityEngine.EventSystems;

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
            ClearPath();
            if (curSelWarrior == value)
                return;

            if (curSelWarrior != null && curSelWarrior.Map != null)
            {
              //  BattleStage.GetAvatarByWarrior(curSelWarrior).IsShowClickFrame = false;
            }

            curSelWarrior = value;
            if (curSelWarrior != null)
            {
              //  BattleStage.GetAvatarByWarrior(curSelWarrior).IsShowClickFrame = true;
                curSelWarrior.MovingPath.Clear();
            }

            ClearPath();

            OnCurrentWarriorChanged?.Invoke();
        }
    }
    Warrior curSelWarrior;
    List<MapTile> pathATKRange = new List<MapTile>();
    List<MapTile> pathMovingRange = new List<MapTile>();

    // 显示攻击范围
    public void ShowAttackRange(float x, float y, Warrior worrior)
    {
        Debug.Assert(pathATKRange.Count == 0, "path range is not empty now.");
        FC.For(worrior.AttackRange.Length, (i) =>
        {
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
                    var avatar = BattleStage.GetAvatarAt(tx, ty);
                    if (avatar != null && avatar.Warrior.Team != worrior.Team) {
                        avatar.AttackHint.SetActive(true);
                    }
                }
            });
        });
    }

    // 清除攻击范围显示
    public void RemoveShowAttackRange()
    {
        FC.For(pathATKRange.Count, (i) =>
        {
            var avatar = BattleStage.GetAvatarAt(pathATKRange[i].X, pathATKRange[i].Y);
            if (avatar != null && avatar.AttackHint.activeSelf)
                avatar.AttackHint.SetActive(false);
        });
        BattleStage.RemoveTile(pathATKRange);
    }

    // 清除可移动范围显示
    public void RemoveMovingPathRange()
    {
        BattleStage.RemoveTile(pathMovingRange);
    }

    BattleStageUI StageUI { get => BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>(); }
   // public ItemOnMap CurrentItem { get; set; }
   // public MapAvatar CurrentAvatar { get; set; }
    public MapTile CurrentTile { get; set; }

    public override void OnClicked(float x, float y)
    {
        WorldPos2MapPos(x, y, out float gx, out float gy);

        // 获取当前点击目标
        var avatar = BattleStage.GetAvatarAt((int)gx, (int)gy);
        var warrior = avatar?.Warrior;
        var item = Room.Battle.Map.GetAt<ItemOnMap>((int)gx, (int)gy);
        var mapTile = BattleStage.GetMapTileAt((int)gx, (int)gy);
        if (CurrentTile != null)
            CurrentTile.IsShowClickFrame = false;
        if(mapTile != null)
            mapTile.IsShowClickFrame = true;
        CurrentTile = mapTile;

        switch (status)
        {
            case "selectingWarrior":

                // 隐藏信息栏
                StageUI.CharacterInfoUI.gameObject.SetActive(false);
                StageUI.obstacleUI.gameObject.SetActive(false);
                StageUI.SkillButtonUI.UpdateItemUsage();

                if (warrior != null)
                {
                    // 显示人物信息栏
                    StageUI.CharacterInfoUI.GetComponent<CharacterInfoUI>().UpdateWarriorInfo(warrior);

                    if (warrior.Team == Room.PlayerMe)
                    {

                        // 根据英雄切换卡牌顺序
                        Room.SortingBattleCards(warrior);

                        // 刷新技能
                        StageUI.SkillButtonUI.UpdateSkillState((BattleStage.Battle as BattlePVE).Energy, warrior);

                        // 选中己方角色，等待攻击指令
                        CurrentSelWarrior = warrior;
                        if (CurrentSelWarrior != null)
                        {
                            status = "selectingAttackTarget";
                            ShowAttackRange(gx, gy, CurrentSelWarrior);
                            //  ShowMovePathRange(gx, gy, (Room.Battle as BattlePVE).AvailableCards.Count);
                        }
                    }
                }
                else if (item != null) // 显示障碍物信息栏
                {
                    StageUI.obstacleUI.UpdateObstacleInfo(item);
                  //  item.MapItem.IsShowClickFrame = true;
                  //  CurrentItem = item;
                }              
                break;
            case "selectingAttackTarget":

                // 显示地形改造按钮
                StageUI.SkillButtonUI.UpdateItemUsage();
               // CurrentAvatar.IsShowClickFrame = false;

                // 再次点击时清除攻击范围显示
                RemoveShowAttackRange();
                RemoveMovingPathRange();
                if (warrior == null)
                {
                    // 点空地
                    if (CurrentSelWarrior.MovingPath.Count >= 2
                        && CurrentSelWarrior.MovingPath[CurrentSelWarrior.MovingPath.Count - 2] == (int)gx
                        && CurrentSelWarrior.MovingPath[CurrentSelWarrior.MovingPath.Count - 1] == (int)gy)
                    {
                        // 如果是移动目的地，则直接移动
                        if (CurrentSelWarrior.MovingPath.Count > 0)
                        {
                            Room.DoMoveOnPath(CurrentSelWarrior);
                        }
                        CurrentSelWarrior = null;
                        status = "selectingWarrior";
                    }
                    else 
                    {
                        if (item != null)// 点障碍物
                        {
                            StageUI.obstacleUI.UpdateObstacleInfo(item);
                          //  item.MapItem.IsShowClickFrame = true;
                          //  CurrentItem = item;
                        }
 
                        // 否则恢复初始状态
                        CurrentSelWarrior = null;
                        status = "selectingWarrior";

                        // 点空地或者障碍物则隐藏人物信息栏
                        StageUI.CharacterInfoUI.gameObject.SetActive(false);
                    }
                }
                else if (warrior.Team == Room.PlayerMe)
                {
                    // 点己方角色，切换操作对象
                    CurrentSelWarrior = warrior;

                    // 显示人物信息栏
                    StageUI.CharacterInfoUI.UpdateWarriorInfo(warrior);
                    StageUI.SkillButtonUI.UpdateSkillState((BattleStage.Battle as BattlePVE).Energy, warrior);

                    // 根据英雄切换卡牌顺序
                    Room.SortingBattleCards(warrior);

                    if (CurrentSelWarrior != null)
                    {
                        ShowAttackRange(gx, gy, CurrentSelWarrior);
                      //  ShowMovePathRange(gx, gy, (Room.Battle as BattlePVE).AvailableCards.Count);
                    }
                   
                }
                else if (CurrentSelWarrior != null)
                {
                    // 显示敌军信息栏
                 //   CurrentAvatar = avatar;
                    StageUI.CharacterInfoUI.UpdateWarriorInfo(warrior);
                    StageUI.SkillButtonUI.UpdateItemUsage();
                    //  CurrentAvatar.IsShowClickFrame = true;

                    // 点对方角色，指定攻击指令

                    // 移动后攻击改为一个单一指令，因为有类似游川影的收刀术需要将其作为一个完整逻辑进行处理
                    if (CurrentSelWarrior.MovingPath.Count > 0 && !CurrentSelWarrior.ActionDone)
                        Room.DoMoveOnPathAndAttack(CurrentSelWarrior, warrior);
                    else if (CurrentSelWarrior.MovingPath.Count > 0) // 仅移动
                        Room.DoMoveOnPath(CurrentSelWarrior);
                    else if (!CurrentSelWarrior.ActionDone) // 仅攻击
                        DoAttack(CurrentSelWarrior, warrior);

                    // 判断是否该刷新技能图片
                    // BattleStage.Battle.SetActionFlag(CurrentSelWarrior, CurrentSelWarrior.ActionDone);
                    CurrentSelWarrior = null;
                    status = "selectingWarrior";
                }
                break;
        }
    }

    // 拖拽连续选择行动路径
    public List<MapTile> pathInSel = new List<MapTile>();
    public override void OnStartDragging(float x, float y)
    {
        WorldPos2MapPos(x, y, out float gx, out float gy);

        // 获取当前点击目标
        var avatar = BattleStage.GetAvatarAt((int)gx, (int)gy);
        var warrior = avatar?.Warrior;
        if (warrior == null || warrior.Moved || warrior.ActionDone || warrior.Team != Room.PlayerMe)
        {
            base.OnStartDragging(x, y);
            return;
        }

        RemoveShowAttackRange();

        // 隐藏点击框
        if (CurrentTile != null)
            CurrentTile.IsShowClickFrame = false;

        // 隐藏信息栏
        StageUI.CharacterInfoUI.gameObject.SetActive(false);
        StageUI.obstacleUI.gameObject.SetActive(false);
        StageUI.SkillButtonUI.UpdateItemUsage();

        // 显示人物信息栏
        StageUI.CharacterInfoUI.UpdateWarriorInfo(warrior);
        StageUI.SkillButtonUI.UpdateSkillState((BattleStage.Battle as BattlePVE).Energy, warrior);

        CurrentSelWarrior = warrior;
        var tile = BattleStage.Tiles[(int)gx, (int)gy];
        ClearPath();
        AddPath(tile);
        status = "selectingPath";
    }

    public override void OnDragging(float fx, float fy, float tx, float ty)
    {
        if (status != "selectingPath")
        {
            base.OnDragging(fx, fy, tx, ty);
            return;
        }

        WorldPos2MapPos(tx, ty, out float gtx, out float gty);
        var tile = BattleStage.Tiles[(int)gtx, (int)gty];
        var tailTile = pathInSel[pathInSel.Count - 1];
        if (tile == tailTile)
            return;

        var secondTailTile = pathInSel.Count < 2 ? null : pathInSel[pathInSel.Count - 2];
        if (tile == secondTailTile)
        {
            // 回退指向尾部第二块，则从路径中退掉尾部第一块
            RemovePath(pathInSel.Count - 1);
            tailTile.Color = MapTile.ColorDefault;
            tailTile.Card = null;
            if (pathInSel.Count > 1) // 头节点不变色显示
            {
                // pathInSel[pathInSel.Count - 1].Color = MapTile.ColorSelectedHead;
                var headTile = pathInSel[pathInSel.Count - 1];
                var tileBeforeHeadTile = pathInSel[pathInSel.Count - 2];
                headTile.SetHeadDir(headTile.X - tileBeforeHeadTile.X, headTile.Y - tileBeforeHeadTile.Y);
            }
        }
        else if (!pathInSel.Contains(tile)) // 指向队列中的中间某一块，则忽略该块
        {
            // 已经达到移动距离上限，也不再增加。起始点不应算在距离限制中，所以 +1
            if (pathInSel.Count >= CurrentSelWarrior.MoveRange + 1)
                return;

            // 拖拽到相邻块则加入路径
            var dist = MU.ManhattanDist(tailTile.X, tailTile.Y, tile.X, tile.Y);
            if (dist == 1 && !Room.Battle.Map.BlockedAt(tile.X, tile.Y, CurrentSelWarrior.StandableTiles))
            {
                if (pathInSel.Count > 1) // 头节点不变色显示
                {
                    var cd = pathInSel[pathInSel.Count - 1];
                    cd.Color = MapTile.ColorDefault;
                    cd.SetDir(tile.X - tailTile.X, tile.Y - tailTile.Y);
                    cd.JudgeCorner(secondTailTile, tailTile, tile);
                    cd.Card = (Room.Battle as BattlePVE).AvailableCards[pathInSel.Count - 2]; // 第一个路径节点并不对应战斗卡牌
                }
                tile.SetHeadDir(tile.X - tailTile.X, tile.Y - tailTile.Y);
               // tile.Color = MapTile.ColorSelectedHead;
                tile.Card = (Room.Battle as BattlePVE).AvailableCards[pathInSel.Count - 1];
               // tile.SetDir(0, 0);
                AddPath(tile);
            }
        }
    }

    public int lastX;
    public int lastY;
    public override void OnEndDragging(float fx, float fy, float tx, float ty)
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
        lastX = pathInSel[pathInSel.Count - 1].X;
        lastY = pathInSel[pathInSel.Count - 1].Y;
        RemoveShowAttackRange();
        ShowAttackRange((float)lastX, (float)lastY, CurrentSelWarrior);
    }

    // 执行攻击指令
    void DoAttack(Warrior attacker, Warrior target)
    {
        Room.DoAttack(attacker, target);
    }

    // 增加路径的时候刷新能量
    public void AddPath(MapTile tile)
    {
        pathInSel.Add(tile);
        RefreshEnergy();
        RefreshBattleCardSelStatus();
    }

    // 减少路径的时候刷新能量
    public void RemovePath(int i)
    {
        pathInSel.RemoveAt(i);
        RefreshEnergy();
        RefreshBattleCardSelStatus();
    }

    // 清除路径的时候刷新能量
    public void ClearPath()
    {
        FC.ForEach(pathInSel, (i, tile) =>
        {
            tile.Color = MapTile.ColorDefault;
            tile.Card = null;
        });

        pathInSel.Clear();
        RefreshEnergy();
        RefreshBattleCardSelStatus();
    }

    // 刷新卡牌选中状态
    public void RefreshBattleCardSelStatus()
    {
        // 路径节点中第一个节点是不算在路径长度内的
        StageUI.CardArea.SetCardSels(pathInSel.Count - 1);
    }

    // 刷新能量
    public void RefreshEnergy()
    {
        var bt = BattleStage.Battle as BattlePVE;

        // 规划的路径上的能量卡先算进来
        int energy = 0;
        if (pathInSel.Count > 0)
        {
            // 路径起始点不消耗卡牌
            FC.For(1, pathInSel.Count, (i) =>
            {
                var c = bt.AvailableCards[i-1] as BattleCardEN;
                energy += c == null ? 0 : c.Energy;
            });
        }

        var en = bt.Energy + energy;
        StageUI.CardArea.RefreshEnergy(en, bt.MaxEnergy);
        StageUI.SkillButtonUI.UpdateSkillState(en,CurrentSelWarrior);
    }

    // 显示可移动的范围
    public void ShowMovePathRange(float x, float y, int availableCardNum) {
        int minX = (x - availableCardNum) < 0 ? 0 : (int)(x - availableCardNum);
        int maxX = (x + availableCardNum) >= BattleStage.Map.Width ? BattleStage.Map.Width : (int)(x + availableCardNum + 1);
        int minY = (y - availableCardNum) < 0 ? 0 : (int)(y - availableCardNum);
        int maxY = (y + availableCardNum) >= BattleStage.Map.Height ? BattleStage.Map.Height : (int)(y + availableCardNum + 1);
        FC.For2(minX, maxX, minY, maxY, (tx, ty) =>
        {
            if (MU.ManhattanDist((int)x, (int)y, tx, ty) <= availableCardNum && BattleStage.GetAvatarAt(tx,ty)==null)
            {
                var tile = BattleStage.CreateMovingMapTile(tx, ty);
                pathMovingRange.Add(tile);
            }
        });
    }
}
