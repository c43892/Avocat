﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

public interface IWithRange
{
    int Range { get; set; }
}

/// <summary>
/// 显示一个指定可选择的位置区域，让玩家在该区域内指定一个位置
/// </summary>
public class PosSelOps : StageOpsLayer
{
    public PosSelOps(BattleStage bs)
           : base(bs)
    {
    }
    List<MapTile> Range = new List<MapTile>();
    IWithRange iRange;
    Action<int, int> onSelPos = null;

    // 显示范围
    public void ShowRange(int cx, int cy, IWithRange iRange, Action< int, int> onSelPos)
    {
        Debug.Assert(Range.Count == 0, "path asasrange is not empty now.");
        this.iRange = iRange;
        this.onSelPos = onSelPos;
        var map = BattleStage.Map;
        FC.SquareFor(cx, cy, iRange.Range, (x, y) =>
        {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
                return;

            if (MU.ManhattanDist(x, y, cx, cy) == iRange.Range)
            {
                var tile = BattleStage.CreateMapTile(x, y);
                Range.Add(tile);
            }
        });

        FC.For(0, Range.Count, i =>
        {
            Range[i].GetComponent<SpriteRenderer>().color = Color.yellow;
            Range[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
        });
    }

    // 检查是否在范围内
    public bool CheckRange(float x, float y, List<MapTile> range)
    {
        foreach (MapTile tile in range)
            if (tile.X == (int)x && tile.Y == (int)y)
                return true;
        return false;
    }

    // 清除范围显示
    public void RemoveShowRange()
    {
        BattleStage.RemoveTile(Range);
    }

    // 返回选择的位置
    public override void OnClicked(float x, float y)
    {
        // 获取当前点击目标
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        var warrior = avatar == null ? null : avatar.Warrior;
        if (warrior == null)
        {
            // 点空地
            if (CheckRange((int)x, (int)y, Range))
            {
                RemoveShowRange();
                BattleStage.InBattleOps.RemoveShowAttackRange();
                onSelPos((int)x, (int)y);
                BattleStage.StartSkillStage(false);
            }
            else
            {
                // 否则恢复初始状态
                RemoveShowRange();
                BattleStage.StartSkillStage(false);
            }
        }
    }
}
