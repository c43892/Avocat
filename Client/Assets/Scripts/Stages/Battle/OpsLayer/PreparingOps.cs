﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;

/// <summary>
/// 准备阶段操作：移动英雄位置
/// </summary>
public class PreparingOps : StageOpsLayer
{
    MapWarrior currentSelAvatar;

    public PreparingOps(BattleStage bs)
        : base(bs)
    {
    }

    public override void OnClicked(float x, float y)
    {
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        if (currentSelAvatar == null && avatar != null)
        {
            // 选中要移动的角色
            currentSelAvatar = avatar;
            avatar.Selected = true;
        }
        else if (currentSelAvatar != null)
        {
            // 交换角色位置
            currentSelAvatar.Selected = false;
            Room.ExchangeWarroirsPosition(currentSelAvatar.X, currentSelAvatar.Y, (int)x, (int)y);
            currentSelAvatar = null;
        }
    }

    public override void OnStartDragging(float x, float y)
    {
        if (currentSelAvatar != null)
        {
            currentSelAvatar.Selected = false;
            currentSelAvatar = null;
        }

        var avatar = BattleStage.Avatars[(int)x, (int)y];
        if (avatar == null)
            return;

        currentSelAvatar = avatar;
        PointerIndicator.gameObject.SetActive(true);
        PointerIndicator.sprite = avatar.SpriteRender.sprite;
        currentSelAvatar.gameObject.SetActive(false);
    }

    public override void OnDragging(float fx, float fy, float cx, float cy)
    {
        if (currentSelAvatar == null)
            return;

        var rootPos = MapRoot.transform.localPosition;
        PointerIndicator.transform.localPosition = new Vector2(rootPos.x + cx, rootPos.y - cy);
    }

    public override void OnEndDragging(float fx, float fy, float cx, float cy)
    {
        if (currentSelAvatar == null)
            return;

        Room.ExchangeWarroirsPosition((int)fx, (int)fy, (int)cx, (int)cy);
        PointerIndicator.gameObject.SetActive(false);
        currentSelAvatar.gameObject.SetActive(true);
        currentSelAvatar = null;
    }
}
