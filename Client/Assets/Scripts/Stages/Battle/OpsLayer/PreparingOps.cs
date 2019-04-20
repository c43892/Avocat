using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;

/// <summary>
/// 准备阶段操作：移动英雄位置
/// </summary>
public class PreparingOps : StageOpsLayer
{
    MapAvatar currentSelAvatar;

    public PreparingOps(BattleStage bs)
        : base(bs)
    {
    }

    public override void OnClicked(float x, float y)
    {
        var avatar = BattleStage.Avatars[(int)x, (int)y];
        if (currentSelAvatar == null && avatar != null && avatar.Warrior.Team == Room.PlayerMe)
        {
            // 选中要移动的角色
            currentSelAvatar = avatar;
            avatar.Color = MapAvatar.ColorSelected;
        }
        else if (currentSelAvatar != null)
        {
            // 交换角色位置
            currentSelAvatar.Color = MapAvatar.ColorDefault;
            Room.DoExchangeWarroirsPosition(currentSelAvatar.X, currentSelAvatar.Y, (int)x, (int)y);
            currentSelAvatar = null;
        }
    }

    public override void OnStartDragging(float x, float y)
    {
        if (currentSelAvatar != null)
        {
            currentSelAvatar.Color = MapAvatar.ColorDefault;
            currentSelAvatar = null;
        }

        var avatar = BattleStage.Avatars[(int)x, (int)y];
        if (avatar == null || avatar.Warrior.Team != Room.PlayerMe)
            return;

        // 显示指针，并隐藏准备拖拽的对象
        currentSelAvatar = avatar;
        PointerIndicator.gameObject.SetActive(true);
        PointerIndicator.sprite = avatar.SpriteRender.sprite;
        currentSelAvatar.gameObject.SetActive(false);
    }

    public override void OnDragging(float fx, float fy, float cx, float cy)
    {
        if (currentSelAvatar == null)
            return;

        // 移动指针
        var rootPos = MapRoot.transform.localPosition;
        PointerIndicator.transform.localPosition = new Vector2(rootPos.x + cx, rootPos.y - cy);
    }

    public override void OnEndDragging(float fx, float fy, float cx, float cy)
    {
        if (currentSelAvatar == null)
            return;

        // 隐藏指针，执行交换操作，并显示被拖拽对象
        Room.DoExchangeWarroirsPosition((int)fx, (int)fy, (int)cx, (int)cy);
        PointerIndicator.gameObject.SetActive(false);
        currentSelAvatar.gameObject.SetActive(true);
        currentSelAvatar = null;
    }
}
