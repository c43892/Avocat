using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Battle;
using System;

/// <summary>
/// 战斗场景
/// </summary>
public class BattleStage : MonoBehaviour
{
    // 相关显示参数
    public int MapTileWidth;
    public int MapTileHeight;

    // 创建地图块模板
    public Func<int, MapTile> MapTileCreator;
    public Func<int, MapWarrior> MapWarriorCreator;

    // 地图显示元素根
    public Transform MapRoot;

    public BattleRoomClient Room { get; private set; }
    public BattleMap Map { get { return Room == null ? null : Room.Battle.Map; } }

    // 创建场景显示对象
    public void BuildBattleStage(BattleRoomClient room)
    {
        ClearMap();

        Room = room;
        BuildMapGrids();
        BuildMapItems();
        BuildWarroirs();

        currentOpLayer = new PreparingOps(this);

        // 英雄
        room.OnWarriorPositionChanged += (int fromX, int fromY, int toX, int toY) =>
        {
            var avFrom = Avatars[fromX, fromY];
            var avTo = Avatars[toX, toY];

            SetAvatarPosition(avFrom, toX, toY);
            SetAvatarPosition(avTo, fromX, fromY);
        };
    }

    void ClearMap()
    {
        Tiles = null;
        Avatars = null;

        while (MapRoot.childCount > 0)
            Destroy(MapRoot.GetChild(0));
    }

    // 构建地块层
    StageOpsLayer currentOpLayer = null;
    public MapTile[,] Tiles { get; private set; }
    void BuildMapGrids()
    {
        Tiles = new MapTile[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var tile = MapTileCreator(Map.Grids[x, y]);
            tile.transform.SetParent(MapRoot);
            tile.X = x;
            tile.Y = y;
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            tile.gameObject.SetActive(true);
            tile.OnClicked += (px, py) => currentOpLayer.OnClicked(px, py);

            Tiles[x, y] = tile;
        });
    }

    // 构建地图道具层
    void BuildMapItems()
    {

    }

    // 构建地图上的角色
    public MapWarrior[,] Avatars { get; private set; }
    void BuildWarroirs()
    {
        Avatars = new MapWarrior[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var warrior = Map.Warriors[x, y];
            if (warrior == null)
                return;

            var avatar = MapWarriorCreator(warrior.Avatar);
            avatar.transform.SetParent(MapRoot);
            var sp = avatar.GetComponent<SpriteRenderer>();
            sp.sortingOrder = 2;
            sp.flipX = warrior.IsOpponent;
            avatar.gameObject.SetActive(true);

            SetAvatarPosition(avatar, x, y);
        });
    }

    // 设置角色位置
    void SetAvatarPosition(MapWarrior avatar, int x, int y)
    {
        if (avatar != null)
        {
            avatar.X = x;
            avatar.Y = y;
        }

        Avatars[x, y] = avatar;
    }
}
