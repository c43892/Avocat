using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

/// <summary>
/// 使用地图道具
/// </summary>
public class UseMapItemOps : StageOpsLayer
{
    MapItem currentSelItem;

    public UseMapItemOps(BattleStage bs)
        : base(bs)
    {
    }

    public override void OnClicked(float x, float y)
    {
        var avatar = BattleStage.Avatars[(int)x, (int)y];

        // 如果已经由选择道具，再次选择地方角色，则对其使用道具
        if (currentSelItem != null && avatar != null && avatar.Warrior.Team != Room.PlayerMe)
        {
            Room.UseItem2(currentSelItem.Item as UsableItem, avatar.Warrior);
            currentSelItem.Color = MapAvatar.ColorDefault;
            currentSelItem = null;
            return;
        }

        // 表示对敌方使用道具，则是其它情况

        var item = BattleStage.Items[(int)x, (int)y];

        if (currentSelItem != null)
        {
            // 切换当前选中道具
            currentSelItem.Color = MapItem.ColorDefault;
            currentSelItem = null;
        }

        if (item != null)
        {
            // 选中要使用的道具
            currentSelItem = item;
            item.Color = MapItem.ColorSelected;
        }
    }

    public override void OnStartDragging(float x, float y)
    {
        if (currentSelItem != null)
        {
            currentSelItem.Color = MapItem.ColorDefault;
            currentSelItem = null;
        }

        var item = BattleStage.Items[(int)x, (int)y];
        if (item == null)
            return;

        // 显示指针，并隐藏准备拖拽的对象
        currentSelItem = item;
        PointerIndicator.SetActive(true);
        PointerIndicator.GetComponentInChildren<TextMesh>().text = item.Item.Name;
        currentSelItem.gameObject.SetActive(false);
    }

    public override void OnDragging(float fx, float fy, float cx, float cy)
    {
        if (currentSelItem == null)
            return;

        // 移动指针
        var rootPos = MapRoot.transform.localPosition;
        PointerIndicator.transform.localPosition = new Vector2(rootPos.x + cx, rootPos.y - cy);
    }

    public override void OnEndDragging(float fx, float fy, float cx, float cy)
    {
        var target = BattleStage.Avatars[(int)cx, (int)cy];
        if (currentSelItem != null && target != null && target.Warrior.Team != Room.PlayerMe)
            Room.UseItem2(currentSelItem.Item as UsableItem, target.Warrior); // 执行操作

        // 隐藏指针
        PointerIndicator.SetActive(false);
        currentSelItem.gameObject.SetActive(true);
        currentSelItem = null;
    }
}
