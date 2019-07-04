using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;
using UnityEngine.UI;
using Swift.Math;

/// <summary>
/// 战斗场景
/// </summary>
public class BattleStage : MonoBehaviour
{
    // 显示操作指针
    public GameObject PointerIndicator;

    // 战斗场景UI
    public GameObject BattleScene;

    // 场景 UI 根节点
    public GameObject BattleStageUIRoot;
    
    // 底层地图操作处理
    public MapGroundLayer MapGround;

    // 地图信息
    public MapReader MapReader;

    // 相关显示参数
    public int MapTileWidth { get; set; }
    public int MapTileHeight { get; set; }

    // 创建地图块模板
    public MapTile MapTile;
    public MapAvatar MapAvatar;
    public MapItem MapItem;

    public Transform SceneOffset; // 处理场景缩放与平移
    public Transform MapRoot; // 地图显示元素根，挂接所有模型光效等
    public Transform SceneBg; // 场景背景图
    public Vector2 SceneBgSize; // 背景图尺寸

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
    public PosSelOps PosSelOps { get; private set; } // 释放技能阶段
    public SkillPreviewOps SkillPreviewOps;

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
        PosSelOps = new PosSelOps(this); //  战斗内释放技能阶段
        SkillPreviewOps = new SkillPreviewOps(this);

        OnTimeBackTriggered = onTimeBackTriggered;

        // 将场景中心移动到屏幕中心
        MapRoot.transform.localPosition = new Vector3(-Map.Width / 2, Map.Height / 2, 0);

        // 将底部响应操作的层铺满整个屏幕
        var sws = SceneWorldSize;
        MapGround.transform.localPosition = new Vector3(-sws.x / 2, sws.y / 2, 0);
        MapGround.transform.localScale = new Vector3(sws.x, sws.y, 1);
    }

    void ClearMap()
    {
        Tiles = null;
        Avatars = null;

        while (MapRoot.childCount > 0)
            DestroyImmediate(MapRoot.GetChild(0).gameObject);
    }

    // 创建maptile用于显示攻击和技能范围
    public MapTile CreateMapTile(float x, float y)
    {
        var tile = Instantiate(MapTile);
        tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/AttackRange");
        tile.GetComponent<SpriteRenderer>().color = Color.red;
        tile.transform.SetParent(MapRoot);
        tile.X = (int)x;
        tile.Y = (int)y;
        tile.transform.localScale = Vector3.one;
        tile.gameObject.name = "AttackRange";
        tile.GetComponent<SpriteRenderer>().sortingOrder = (int)y + 3;
        tile.gameObject.SetActive(true);
        return tile;
    }

    // 创建maptile用于显示可移动范围
    public MapTile CreateMovingMapTile(float x, float y)
    {
        var tile = Instantiate(MapTile);
        tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/Moving");
        tile.transform.SetParent(MapRoot);
        tile.X = (int)x;
        tile.Y = (int)y;
        tile.transform.localScale = Vector3.one;
        tile.gameObject.name = "MovingRange";
        tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
            Map.SetMapTileInfo(tile);
            SetMapTile(tile);
            tile.GetComponent<SpriteRenderer>().sortingOrder = y + 1;
            tile.gameObject.SetActive(true);
            Tiles[x, y] = tile;
        });
    }

    public void SetMapTile(MapTile tile)
    {
        var MapData = tile.MapData;
        var material = tile.transform.Find("Material").GetComponent<SpriteRenderer>();
        if (MapData.Type != TileType.None)
            material.sprite = Resources.Load<Sprite>("UI/MapTile/" + MapData.Type.ToString());

        // 设置地块渲染位置
        material.sortingOrder = MapData.MaterialSortingOrder;

        // 设置出生点逻辑和表现
        if (MapData.RespawnForChamp == true || MapData.RespawnForEnemy == true)
        {
            tile.transform.Find("RespawnPlace").gameObject.SetActive(true);

            // 设置出生点渲染位置
            var Mesh = tile.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
            Mesh.sortingOrder = MapData.RespawnSortingOrder;
        }
    }

    // 构建地图上的对象
    public MapItem[,] Items { get; private set; }
    void BuildMapItems()
    {
        Items = new MapItem[Map.Width, Map.Height];
        Map.ForeachObjs<Avocat.ItemOnMap>((x, y, item) => CreateMapItem(x, y, item));
    }

    // 构建地图上的角色
    public MapAvatar[,] Avatars { get; private set; }
    void BuildAvatars()
    {
        Avatars = new MapAvatar[Map.Width, Map.Height];
        Map.ForeachObjs<Warrior>((x, y, target) => CreateWarriorAvatar(x, y, target));
    }

    // 创建角色对应的显示对象
    public void CreateWarriorAvatar(int x, int y, Warrior warrior)
    {
        var avatar = Instantiate(MapAvatar);
        avatar.Warrior = warrior;
        avatar.BattleStage = this;
        avatar.transform.SetParent(MapRoot);
        avatar.gameObject.name = warrior.ID;
        avatar.SetIdleAnimation(warrior);
        avatar.gameObject.SetActive(true);
        avatar.RefreshAttrs();
        SetAvatarPosition(avatar, x, y);
    }

    // 创建道具对应的显示对象
    public void CreateMapItem(int x, int y, BattleMapObj item)
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

    // 获取指定位置 Avatar
    public MapAvatar GetAvatarAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Avatars.GetLength(0) || y >= Avatars.GetLength(1))
            return null;

        return Avatars[x, y];
    }

    // 获取指定位置 MapTile
    public MapTile GetMapTileAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Avatars.GetLength(0) || y >= Avatars.GetLength(1))
            return null;

        return Tiles[x, y];
    }

    // 根据道具获取 MapItem
    public MapItem GetMapItemByItem(BattleMapObj item)
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

    public void ForeachMapTile(Action<int, int, MapTile> act, Func<bool> continueCondition = null)
    {
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var tile = Tiles[x, y];
            if (tile == null)
                return;

            act(x, y, tile);
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
    public void StartSkillStage(bool enterOrExit)
    {
        if (enterOrExit)
        {
            CurrentOpLayer = PosSelOps;
            BattleStageUIRoot.GetComponent<BattleStageUI>().SkillButtonUI.HideButton();
        }
        else
        {
            CurrentOpLayer = InBattleOps;
            BattleStageUIRoot.GetComponent<BattleStageUI>().SkillButtonUI.ShowButton();
        }
    }

    public void StartSkill(int cx, int cy, ActiveSkill skill, Action<int, int> onSelPos)
    {
        PosSelOps.ShowRange(cx, cy, skill, onSelPos);
        InBattleOps.RemoveShowAttackRange();
    }

    // 挂接地图操作逻辑
    void SetupGroundOps()
    {
        MapGround.OnClicked += (float x, float y) => CurrentOpLayer.OnClicked(x, y);
        MapGround.OnStartDragging += (float x, float y) => CurrentOpLayer.OnStartDragging(x, y);
        MapGround.OnDragging += (float fx, float fy, float cx, float cy) => CurrentOpLayer.OnDragging(fx, fy, cx, cy);
        MapGround.OnEndDragging += (float fx, float fy, float cx, float cy) => CurrentOpLayer.OnEndDragging(fx, fy, cx, cy);
        MapGround.OnStartScaling += () => CurrentOpLayer.OnStartScaling();
        MapGround.OnScaling += (float scale, float cx, float cy) => CurrentOpLayer.OnScaling(scale, cx, cy);

        // 地图缩放移动等
        StageOpsLayer.DefaultStartDraggingHandler += StartDraggingMap;
        StageOpsLayer.DefaultDraggingHandler += DraggingMap;
        StageOpsLayer.DefaultStartScaling += StartScaling;
        StageOpsLayer.DefaultScaling += Scaling;
    }

    #region 地图平移缩放

    float fromScale;
    Vector3 fromPos;

    void StartDraggingMap(float x, float y)
    {
        fromPos = SceneOffset.localPosition;
    }

    // 屏幕显示范围对应的世界坐标范围
    Vector2 SceneWorldSize
    {
        get
        {
            var min = Camera.main.ViewportToWorldPoint(Vector3.zero);
            var max = Camera.main.ViewportToWorldPoint(Vector3.one);
            return max - min;
        }
    }

    void DraggingMap(float fx, float fy, float tx, float ty)
    {
        var offset = new Vector2(tx - fx, ty - fy);
        SceneOffset.localPosition = new Vector3(fromPos.x + offset.x, fromPos.y + offset.y, fromPos.z);
        AdjustOffset();
    }

    void StartScaling()
    {
        fromScale = SceneOffset.localScale.x;
    }

    void Scaling(float scale, float cx, float cy)
    {
        var s = fromScale * scale;
        if (s > 2) return;
        var sx = SceneWorldSize.x / SceneBgSize.x;
        var sy = SceneWorldSize.y / SceneBgSize.y;
        var sMin = sx > sy ? sx : sy;
        if (s < sMin) return;

        CurrentOpLayer.WorldPos2MapPos(cx, cy, out float scx, out float scy);
        SceneOffset.localScale = new Vector3(fromScale * scale, fromScale * scale, 1);
        CurrentOpLayer.MapPos2WorldPos(scx, scy, out float cx2, out float cy2);
        SceneOffset.localPosition += new Vector3(cx - cx2, cy - cy2, 0);
        AdjustOffset();
    }

    void AdjustOffset()
    {
        // 限制缩放范围
        var s = SceneOffset.localScale.x;
        
        SceneOffset.localScale = new Vector3(s, s, 1);

        // 限制移动范围
        var x = SceneOffset.localPosition.x;
        var y = SceneOffset.localPosition.y;
        var bgs = SceneBgSize * s;

        var left = (SceneWorldSize.x - bgs.x) / 2;
        var right = -left;
        x = x.Clamp(left, right);

        var top = (SceneWorldSize.y - bgs.y) / 2;
        var bottom = -top;
        y = y.Clamp(top, bottom);
        SceneOffset.localPosition =
            new Vector3(x, y, fromPos.z);
    }

    #endregion
}
