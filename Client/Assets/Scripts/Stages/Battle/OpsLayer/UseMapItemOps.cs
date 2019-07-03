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
        WorldPos2MapPos(x, y, out float gx, out float gy);
        var avatar = BattleStage.Avatars[(int)gx, (int)gy];

        // 如果已经由选择道具，再次选择地方角色，则对其使用道具
        if (currentSelItem != null && avatar != null && avatar.Warrior.Team != Room.PlayerMe)
        {
            Room.UseItem2(currentSelItem.Item as Avocat.ItemOnMap, avatar.Warrior);
            currentSelItem.Color = MapItem.ColorDefault;
            currentSelItem = null;
            return;
        }

        // 表示对敌方使用道具，则是其它情况

        var item = BattleStage.Items[(int)gx, (int)gy];

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

        WorldPos2MapPos(x, y, out float gx, out float gy);
        var item = BattleStage.Items[(int)gx, (int)gy];
        if (item == null)
            return;

        // 显示指针，并隐藏准备拖拽的对象
        currentSelItem = item;
        PointerIndicator.SetActive(true);
        PointerIndicator.GetComponentInChildren<TextMesh>().text = item.Item.ID;
        currentSelItem.gameObject.SetActive(false);
    }

    public override void OnDragging(float fx, float fy, float tx, float ty)
    {
        if (currentSelItem == null)
            return;

        // 移动指针
        WorldPos2MapPos(tx, ty, out float tgx, out float tgy);
        var rootPos = MapRoot.transform.localPosition;
        PointerIndicator.transform.localPosition = new Vector2(rootPos.x + tgx, rootPos.y - tgy);
    }

    public override void OnEndDragging(float fx, float fy, float tx, float ty)
    {
        WorldPos2MapPos(tx, ty, out float tgx, out float tgy);
        var target = BattleStage.Avatars[(int)tgx, (int)tgy];
        if (currentSelItem != null && target != null && target.Warrior.Team != Room.PlayerMe)
            Room.UseItem2(currentSelItem.Item as Avocat.ItemOnMap, target.Warrior); // 执行操作

        // 隐藏指针
        PointerIndicator.SetActive(false);
        if (currentSelItem != null)
            currentSelItem.gameObject.SetActive(true);

        currentSelItem = null;
    }
}
