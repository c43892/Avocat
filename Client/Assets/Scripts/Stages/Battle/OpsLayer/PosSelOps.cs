using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

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
    Action<int, int> onSelPos = null;

    // 显示范围
    public void ShowRange(int cx, int cy, ActiveSkill skill, Action< int, int> onSelPos)
    {
        Debug.Assert(Range.Count == 0, "path asasrange is not empty now.");
        this.onSelPos = onSelPos;
        var map = BattleStage.Map;

        // 如果无释放范围的技能
        if (!(skill is ISkillWithRange))
        {
            // 需要过滤目标
            if (skill is ISkillWithTargetFilter)
            {
                var targetList = (skill as ISkillWithTargetFilter).AllAvaliableTargets();
                FC.For(targetList.Count, (i) =>
                {
                    var warrior = targetList[i] as Warrior;
                    if (warrior != null)
                    {
                        // 建立用于显示技能范围的地块
                        var avatar = BattleStage.GetAvatarByWarrior(warrior);
                        var mapTile = BattleStage.CreateMapTile(avatar.X, avatar.Y);
                        mapTile.gameObject.SetActive(false);
                        Range.Add(mapTile);

                        // 显示人物身上的技能标识
                        if (warrior.Team == skill.Owner.Team)
                            avatar.FriendSkillDec.SetActive(true);
                        else
                            avatar.EnemySkillDec.SetActive(true);
                    }
                    else
                        Debug.Assert(warrior != null, "the target is not warrior");
                });
            }
        }
        else // 有释放范围的技能
        {
            var range = (skill as ISkillWithRange).Range;
            FC.SquareFor(cx, cy, range, (x, y) =>
            {
                if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
                   return;
                var obj = map.GetAt<BattleMapObj>(x, y);
                if (MU.ManhattanDist(x, y, cx, cy) == range)
                {
                    if (obj == null || (obj != null && obj is Hero))
                    {
                        var tile = BattleStage.CreateMapTile(x, y);
                        Range.Add(tile);
                    }
                }
            });
        } 
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
        BattleStage.Map.ForeachObjs((x, y, obj) =>
        {
            if (obj is Warrior)
            {
                var avatar = BattleStage.GetAvatarByWarrior(obj as Warrior);
                avatar.FriendSkillDec.SetActive(false);
                avatar.EnemySkillDec.SetActive(false);
            }
        });
    }

    // 返回选择的位置
    public override void OnClicked(float x, float y)
    {
        WorldPos2MapPos(x, y, out float gx, out float gy);

        // 检查是否在技能范围内
        if (CheckRange((int)gx, (int)gy, Range))
        {
            RemoveShowRange();
            BattleStage.InBattleOps.RemoveShowAttackRange();
            onSelPos((int)gx, (int)gy);
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
