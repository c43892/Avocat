using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

public class SkillsOps : StageOpsLayer
{
    public SkillsOps(BattleStage bs)
           : base(bs)
    {
    }
    bool enterSkillStage; 
    public ActiveSkill currentSkill;
    List<MapTile> SkillRange = new List<MapTile>();
    IWithRange Skill;
    Action<int, int> onSelPos = null;

    // 显示炮塔释放范围
    public void ShowSkillAttackRange(float x, float y, IWithRange skill, Action< int, int> onSelPos)
    {
        Skill = skill;
        this.onSelPos = onSelPos;
        Debug.Assert(SkillRange.Count == 0, "path asasrange is not empty now.");
        // MU.ManhattanDist(x, y, tx, ty) <= AttackRange
        int minX = (x - skill.Range) < 0 ? 0 : (int)(x - skill.Range);
        int maxX = (x + skill.Range) >= BattleStage.Map.Width ? BattleStage.Map.Width : (int)(x + skill.Range + 1);
        int minY = (y - skill.Range) < 0 ? 0 : (int)(y - skill.Range);
        int maxY = (y + skill.Range) >= BattleStage.Map.Height ? BattleStage.Map.Height : (int)(y + skill.Range + 1);
        FC.For2(minX, maxX, minY, maxY, (tx, ty) =>
        {
            if (MU.ManhattanDist((int)x, (int)y, tx, ty) == skill.Range)
            {
                var tile = BattleStage.CreateMapTile(tx, ty);
                SkillRange.Add(tile);
            }
        });

        FC.For(0, SkillRange.Count, i =>
        {
            SkillRange[i].GetComponent<SpriteRenderer>().color = Color.yellow;
            SkillRange[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
        });
    }

    // 检查是否在释放技能范围内
    public bool CheckSkillRange(float x, float y, List<MapTile> SkillRange)
    {
        foreach (MapTile tile in SkillRange)
        {
            if (tile.X == (int)x && tile.Y == (int)y)
            {
                return true;
            }
        }
        return false;
    }

    // 清除释放技能范围显示
    public void RemoveShowSkillRange()
    {
        BattleStage.RemoveTile(SkillRange);
    }

    // 返回释放技能的位置

    public override void OnClicked(float x, float y)
    {
        // 获取当前点击目标
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        var warrior = avatar == null ? null : avatar.Warrior;

        if (warrior == null)
        {
            // 点空地
            if (CheckSkillRange((int)x, (int)y, SkillRange))
            {
                RemoveShowSkillRange();
                onSelPos((int)x, (int)y);
            }
            else
            {
                // 否则恢复初始状态
                RemoveShowSkillRange();
                enterSkillStage = false;
                BattleStage.StartSkillStage(enterSkillStage);
            }
        }
    }
}
