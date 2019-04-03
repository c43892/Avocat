using System;
using Swift;
using UnityEngine;

/// <summary>
/// 战斗内不同场景操作逻辑
/// </summary>
public class StageOpsLayer
{
    public BattleStage BattleStage { get; set; }

    public StageOpsLayer(BattleStage bs)
    {
        BattleStage = bs;
    }

    public virtual void OnClicked(int x, int y)
    {

    }
}
