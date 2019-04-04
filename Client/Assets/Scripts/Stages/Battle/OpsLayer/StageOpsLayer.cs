using System;
using Swift;
using UnityEngine;

/// <summary>
/// 战斗内不同场景操作逻辑
/// </summary>
public class StageOpsLayer
{
    public BattleStage BattleStage { get; set; }
    public BattleRoomClient Room { get { return BattleStage.Room; } }
    public SpriteRenderer PointerIndicator { get { return BattleStage.PointerIndicator; } }
    public Transform MapRoot { get { return BattleStage.MapRoot; } }

    public StageOpsLayer(BattleStage bs)
    {
        BattleStage = bs;
    }

    public virtual void OnClicked(float x, float y)
    {
    }

    public virtual void OnStartDragging(float x, float y)
    {
    }

    public virtual void OnDragging(float fx, float fy, float cx, float cy)
    {
    }

    public virtual void OnEndDragging(float fx, float fy, float cx, float cy)
    {
    }
}
