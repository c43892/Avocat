using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

/// <summary>
/// 战斗场景
/// </summary>
public class BattleStage : MonoBehaviour
{
    // 显示操作指针
    public SpriteRenderer PointerIndicator;

    // 底层地图操作处理
    public MapGround MapGround;

    // 相关显示参数
    public int MapTileWidth { get; set; }
    public int MapTileHeight { get; set; }

    // 创建地图块模板
    public Func<int, MapTile> MapTileCreator;
    public Func<int, MapWarrior> MapWarriorCreator;

    // 地图显示元素根
    public Transform MapRoot;

    public BattleRoomClient Room { get; private set; }
    public BattleMap Map { get { return Room == null ? null : Room.Battle.Map; } }

    private void Awake()
    {
        // 所有操作转交当前操作层逻辑
        MapGround.OnClicked += (float x, float y) => currentOpLayer.OnClicked((int)x, (int)y);
        MapGround.OnStartDragging += (float x, float y) => currentOpLayer.OnStartDragging(x, y);
        MapGround.OnDragging += (float fx, float fy, float cx, float cy) => currentOpLayer.OnDragging(fx, fy, cx, cy);
        MapGround.OnEndDragging += (float fx, float fy, float cx, float cy) => currentOpLayer.OnEndDragging(fx, fy, cx, cy);
    }

    // 创建场景显示对象
    public void BuildBattleStage(BattleRoomClient room)
    {
        ClearMap();

        Room = room;

        BuildMapGrids();
        BuildMapItems();
        BuildWarroirs();

        MapGround.Area = new Rect(MapRoot.transform.localPosition.x, MapRoot.transform.localPosition.y, Map.Width, Map.Height);

        currentOpLayer = new PreparingOps(this);

        // 英雄位置变化
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
            var sp = avatar.SpriteRender;
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
