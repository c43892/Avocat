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

    public static Action<float, float> DefaultStartDraggingHandler = null;
    public static Action<float, float, float, float> DefaultDraggingHandler = null;
    public static Action<float, float, float, float> DefaultEndDraggingHandler = null;
    public static Action DefaultStartScaling = null;
    public static Action<float, float, float> DefaultScaling = null;

    public StageOpsLayer(BattleStage bs)
    {
        BattleStage = bs;
    }

    public void WorldPos2MapPos(float x, float y, out float tx, out float ty)
    {
        var p = MapRoot.transform.worldToLocalMatrix.MultiplyPoint(new Vector2(x, y));
        tx = p.x;
        ty = p.y;
    }

    public void MapPos2WorldPos(float x, float y, out float tx, out float ty)
    {
        var p = MapRoot.transform.localToWorldMatrix.MultiplyPoint(new Vector2(x, y));
        tx = p.x;
        ty = p.y;
    }

    public virtual void OnClicked(float x, float y)
    {
    }

    // 场景平移

    public virtual void OnStartDragging(float x, float y)
    {
        DefaultStartDraggingHandler.SC(x, y);
    }

    public virtual void OnDragging(float fx, float fy, float tx, float ty)
    {
        DefaultDraggingHandler.SC(fx, fy, tx, ty);
    }

    public virtual void OnEndDragging(float fx, float fy, float tx, float ty)
    {
        DefaultEndDraggingHandler.SC(fx, fy, tx, ty);
    }

    // 场景缩放

    public virtual void OnStartScaling()
    {
        DefaultStartScaling.SC();
    }

    public virtual void OnScaling(float scale, float cx, float cy)
    {
        DefaultScaling.SC(scale, cx, cy);
    }
}
