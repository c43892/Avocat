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
    public GameObject PointerIndicator;

    public GameObject BattleStageUIRoot;
    public GameObject actionDone;
    public GameObject energy;

    // 底层地图操作处理
    public MapGroundLayer MapGround;

    // 相关显示参数
    public int MapTileWidth { get; set; }
    public int MapTileHeight { get; set; }

    // 创建地图块模板
    public MapTile MapTile;
    public MapAvatar MapAvatar;
    public MapItem MapItem;

    // 地图显示元素根
    public Transform MapRoot;

    public BattleRoomClient Room { get; private set; }
    public Battle Battle { get { return Room?.Battle; } }
    public BattleMap Map { get { return Room?.Battle.Map; } }

    private void Awake()
    {
        SetupGroundOps(); // 所有操作转交当前操作层逻辑
    }
    
    public void Clear()
    {
        ClearMap();
    }

    public PreparingOps PreparingOps { get; private set; }  // 准备阶段
    public InBattleOps InBattleOps { get; private set; } // 战斗内一般阶段
    public UseMapItemOps UseMapItemOps { get; private set; }  // 战斗内地形改造阶段
    public PosSelOps SkillOps { get; private set; }

    public Action<BattleReplay> OnTimeBackTriggered { get; private set; }

    // 创建场景显示对象
    public void Build(BattleRoomClient room, Action<BattleReplay> onTimeBackTriggered)
    {
        ClearMap();

        Room = room;

        BuildMapGrids();
        BuildMapItems();
        BuildAvatars();

        PreparingOps = new PreparingOps(this); // 准备阶段
        InBattleOps = new InBattleOps(this); // 战斗内一般阶段
        UseMapItemOps = new UseMapItemOps(this); // 战斗内地形改造阶段
        SkillOps = new PosSelOps(this); //  战斗内释放技能阶段
        MapGround.Area = new Rect(MapRoot.transform.localPosition.x, MapRoot.transform.localPosition.y, Map.Width, Map.Height);
        OnTimeBackTriggered = onTimeBackTriggered;
    }

    void ClearMap()
    {
        Tiles = null;
        Avatars = null;

        while (MapRoot.childCount > 0)
            DestroyImmediate(MapRoot.GetChild(0).gameObject);
    }

    // 创建maptile用于显示攻击范围
    public MapTile CreateMapTile(float x, float y)
    {
        var tile = Instantiate(MapTile);
        tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/MapTile");
        tile.transform.SetParent(MapRoot);
        tile.X = (int)x;
        tile.Y = (int)y;
        tile.gameObject.name = "Range";
        tile.GetComponent<SpriteRenderer>().sortingOrder = 2;
        tile.gameObject.SetActive(true);
        return tile;
    }

    // 摧毁用于显示攻击范围的格子
    public void RemoveTile(List<MapTile> pathATKRange)
    {
        foreach (MapTile map in pathATKRange)
            Destroy(map.gameObject);

        pathATKRange.Clear();
    }

    // 构建地块层
    public StageOpsLayer CurrentOpLayer { get; private set; }
    public MapTile[,] Tiles { get; private set; }
    void BuildMapGrids()
    {
        Tiles = new MapTile[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var tile = Instantiate(MapTile);
            tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/MapTile");
            tile.transform.SetParent(MapRoot);
            tile.X = x;
            tile.Y = y;
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            tile.gameObject.SetActive(true);

            Tiles[x, y] = tile;
        });
    }

    // 构建地图道具层
    public MapItem[,] Items { get; private set; }
    void BuildMapItems()
    {
        Items = new MapItem[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var item = Map.GetItemAt(x, y);
            if (item == null)
                return;

            CreateMapItem(x, y, item);
        });
    }

    // 构建地图上的角色
    public MapAvatar[,] Avatars { get; private set; }
    void BuildAvatars()
    {
        Avatars = new MapAvatar[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var warrior = Map.GetWarriorAt(x, y);
            if (warrior == null)
                return;

            CreateWarriorAvatar(x, y, warrior);
        });
    }

    // 创建角色对应的显示对象
    public void CreateWarriorAvatar(int x, int y, Warrior warrior)
    {
        var avatar = Instantiate(MapAvatar);
        avatar.Warrior = warrior;
        avatar.BattleStage = this;
        avatar.transform.SetParent(MapRoot);

        avatar.gameObject.SetActive(true);
        avatar.RefreshAttrs();
        SetAvatarPosition(avatar, x, y);
    }

    // 创建道具对应的显示对象
    public void CreateMapItem(int x, int y, BattleMapItem item)
    {
        var mapItem = Instantiate(MapItem);
        mapItem.Item = item;
        mapItem.BattleStage = this;
        mapItem.transform.SetParent(MapRoot);

        mapItem.gameObject.SetActive(true);
        mapItem.RefreshAttrs();
        SetItemPosition(mapItem, x, y);
    }

    // 根据角色获取 Avatar
    public MapAvatar GetAvatarByWarrior(Warrior warrior)
    {
        MapAvatar avatar = null;
        ForeachAvatar((x, y, a) =>
        {
            if (a != null && a.Warrior == warrior)
                avatar = a;
        }, () => avatar == null);

        Debug.Assert(avatar != null, "warrior should have a avatar in battle map");
        return avatar;
    }

    // 根据道具获取 MapItem
    public MapItem GetMapItemByItem(BattleMapItem item)
    {
        MapItem mapItem = null;
        ForeachItem((x, y, a) =>
        {
            if (a != null && a.Item == item)
                mapItem = a;
        }, () => mapItem == null);

        Debug.Assert(mapItem != null, "item should have a mapitem in battle map");
        return mapItem;
    }

    public void ForeachAvatar(Action<int, int, MapAvatar> act, Func<bool> continueCondition = null)
    {
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var avatar = Avatars[x, y];
            if (avatar == null)
                return;

            act(x, y, avatar);
        }, continueCondition);
    }

    public void ForeachItem(Action<int, int, MapItem> act, Func<bool> continueCondition = null)
    {
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var item = Items[x, y];
            if (item == null)
                return;

            act(x, y, item);
        }, continueCondition);
    }

    // 设置角色位置
    public void SetAvatarPosition(MapAvatar avatar, int x, int y)
    {
        if (avatar != null)
        {
            avatar.X = x;
            avatar.Y = y;
        }

        Avatars[x, y] = avatar;
    }

    // 设置道具位置
    public void SetItemPosition(MapItem item, int x, int y)
    {
        if (item != null)
        {
            item.X = x;
            item.Y = y;
        }

        Items[x, y] = item;
    }

    // 开始战斗准备阶段
    public void StartPreparing()
    {
        CurrentOpLayer = PreparingOps;
    }

    // 开始战斗阶段
    public void StartFighting()
    {
        CurrentOpLayer = InBattleOps;
    }

    // 开始/退出地形改造
    public void StartUseItem(bool enterOrExit)
    {
        if (enterOrExit)
            CurrentOpLayer = UseMapItemOps;
        else
            CurrentOpLayer = InBattleOps;
    }

    // 判断是否进入释放技能阶段
    public void StartSkillStage(bool enterOrExit) {
        if (enterOrExit)
        {
            CurrentOpLayer = SkillOps;
            HideButton();
        }
        else {
            CurrentOpLayer = InBattleOps;
            GetButton();
        }
    }

    // 隐藏energy和Done按钮
    public void HideButton()
    {
        actionDone = GameObject.Find("ActionDone");
        energy = GameObject.Find("Energy");
        actionDone.SetActive(false);
        energy.SetActive(false);
    }

// 恢复energy和Done按钮
    public void GetButton()
    {     
        actionDone.SetActive(true);
        energy.SetActive(true);
    }

    public void StartSkill(int cx, int cy, IWithRange skill, Action<int, int> onSelPos)
    {

        SkillOps.ShowRange(cx, cy, skill, onSelPos);
        
    }

    // 挂接地图操作逻辑
    void SetupGroundOps()
    {
        MapGround.OnClicked += (float x, float y) => CurrentOpLayer.OnClicked((int)x, (int)y);
        MapGround.OnStartDragging += (float x, float y) => CurrentOpLayer.OnStartDragging(x, y);
        MapGround.OnDragging += (float fx, float fy, float cx, float cy) => CurrentOpLayer.OnDragging(fx, fy, cx, cy);
        MapGround.OnEndDragging += (float fx, float fy, float cx, float cy) => CurrentOpLayer.OnEndDragging(fx, fy, cx, cy);
    }
}
