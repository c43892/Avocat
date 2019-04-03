using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;

/// <summary>
/// 准备阶段操作：移动英雄位置
/// </summary>
public class PreparingOps : StageOpsLayer
{
    MapWarrior currentSelAvatar;

    BattleRoomClient Room { get { return BattleStage.Room; } }

    public PreparingOps(BattleStage bs)
        : base(bs)
    {

    }

    public override void OnClicked(int x, int y)
    {
        var avatar = BattleStage.Avatars[x, y];
        if (currentSelAvatar == null && avatar != null)
        {
            // 选中要移动的角色
            currentSelAvatar = avatar;
            avatar.Selected = true;
        } else if (currentSelAvatar != null)
        {
            // 交换角色位置
            currentSelAvatar.Selected = false;
            Room.ExchangeWarroirsPosition(currentSelAvatar.X, currentSelAvatar.Y, x, y);
            currentSelAvatar = null;
        }
    }
}
