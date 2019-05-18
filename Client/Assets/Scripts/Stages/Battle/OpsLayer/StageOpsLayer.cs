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

    public void WorldPos2ScenePos(float x, float y, out float tx, out float ty)
    {
        var scale = SceneOffset.transform.localScale;
        tx = (x - SceneOffset.transform.position.x) / scale.x;
        ty = (SceneOffset.transform.position.y - y) / scale.y;
    }

    public void ScenePos2WorldPos(float x, float y, out float tx, out float ty)
    {
        var scale = SceneOffset.transform.localScale;
        tx = x * scale.x + SceneOffset.transform.position.x;
        ty = -y * scale.y + SceneOffset.transform.position.y;
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
            new Vector3(fromPos.x + offset.x, fromPos.y + offset.y, fromPos.z);
    }

    public virtual void OnEndDragging(float fx, float fy, float cx, float cy)
    {
    }

    // 场景缩放

    float s;
    public virtual void OnStartScaling()
    {
        s = SceneOffset.transform.localScale.x;
    }

    public virtual void OnScaling(float scale, float cx, float cy)
    {
        WorldPos2ScenePos(cx, cy, out float scx, out float scy);
        SceneOffset.transform.localScale = new Vector3(s * scale, s * scale, 1);
        ScenePos2WorldPos(scx, scy, out float cx2, out float cy2);
        SceneOffset.transform.localPosition += new Vector3(cx - cx2, cy - cy2, 0);
    }
}
