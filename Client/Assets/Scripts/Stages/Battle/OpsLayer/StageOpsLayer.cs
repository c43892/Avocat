using System;
using Swift;
using UnityEngine;

/// <summary>
/// 战斗内不同场景操作逻辑
/// </summary>
public class StageOpsLayer
{
    public BattleStage BattleStage { get; set; }
    public BattleRoomClient Room { get => BattleStage.Room; } 
    public GameObject PointerIndicator { get => BattleStage.PointerIndicator; }
    public Transform MapRoot { get => BattleStage.MapRoot; } 
    public Transform SceneOffset { get => BattleStage.SceneOffset; } 

    public StageOpsLayer(BattleStage bs)
    {
        BattleStage = bs;
    }

    public virtual void OnClicked(float x, float y)
    {
    }

    // 场景平移

    Vector3 fromPos;
    public virtual void OnStartDragging(float x, float y)
    {
        fromPos = SceneOffset.transform.localPosition;
    }

    public virtual void OnDragging(float fx, float fy, float tx, float ty)
    {
        var offset = new Vector2(tx - fx, ty - fy);
        SceneOffset.transform.localPosition =
            new Vector3(fromPos.x + offset.x, fromPos.y - offset.y, fromPos.z);
    }

    public virtual void OnEndDragging(float fx, float fy, float cx, float cy)
    {
    }

    // 场景缩放

    public virtual void OnScaling(float scale)
    {
    }
}
