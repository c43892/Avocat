using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Spine.Unity;

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
        WorldPos2MapPos(x, y, out float gx, out float gy);

        var avatar = BattleStage.GetAvatarAt((int)gx, (int)gy);
        if (currentSelAvatar == null && avatar != null && avatar.Warrior.Team == Room.PlayerMe)
        {
            // 选中要移动的角色
            currentSelAvatar = avatar;
            avatar.IsShowClickFrame = true;
        }
        else if (currentSelAvatar != null)
        {
            // 交换角色位置
            currentSelAvatar.IsShowClickFrame = false;
            Room.DoExchangeWarroirsPosition(currentSelAvatar.X, currentSelAvatar.Y, (int)gx, (int)gy);
            currentSelAvatar = null;
        }

    }
    Vector2 offset;
    public override void OnStartDragging(float x, float y)
    {
        if (currentSelAvatar != null)
        {
            currentSelAvatar.Color = MapAvatar.ColorDefault;
            currentSelAvatar = null;
        }

        WorldPos2MapPos(x, y, out float gx, out float gy);
        var avatar = BattleStage.GetAvatarAt((int)gx, (int)gy);
        if (avatar == null || avatar.Warrior.Team != Room.PlayerMe)
        {
            base.OnStartDragging(x, y);
            return;
        }

        // 显示指针，并隐藏准备拖拽的对象
        currentSelAvatar = avatar;
        PointerIndicator.SetActive(true);
       // PointerIndicator.GetComponentInChildren<TextMesh>().text = avatar.Warrior.DisplayName;
        PointerIndicator.GetComponent<PoniterIndicator>().SetIdleAnimation(currentSelAvatar);
       // WorldPos2ScenePos(x, y, out float gtx, out float gty);
       // PointerIndicator.transform.localPosition = new Vector2(gtx, -gty);
        PointerIndicator.transform.position = currentSelAvatar.transform.position;
        offset = new Vector2(PointerIndicator.transform.position.x-x,PointerIndicator.transform.position.y -y);
        currentSelAvatar.gameObject.SetActive(false);
    }

    public override void OnDragging(float fx, float fy, float tx, float ty)
    {
        if (currentSelAvatar == null)
        {
            base.OnDragging(fx, fy, tx, ty);
            return;
        }


        // 移动指针
        WorldPos2MapPos(tx, ty, out float gtx, out float gty);
        PointerIndicator.transform.localPosition = new Vector2(gtx, -gty)+offset;
    }

    public override void OnEndDragging(float fx, float fy, float tx, float ty)
    {
        WorldPos2MapPos(fx, fy, out float gfx, out float gfy);
        WorldPos2MapPos(tx, ty, out float gtx, out float gty);

        if (currentSelAvatar == null)
            return;

        // 隐藏指针，执行交换操作，并显示被拖拽对象
        Room.DoExchangeWarroirsPosition((int)gfx, (int)gfy, (int)gtx, (int)gty);
        PointerIndicator.SetActive(false);
        currentSelAvatar.gameObject.SetActive(true);
        currentSelAvatar = null;
    }
}
