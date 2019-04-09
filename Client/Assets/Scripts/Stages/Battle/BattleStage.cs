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
    public MapGroundLayer MapGround;

    // 相关显示参数
    public int MapTileWidth { get; set; }
    public int MapTileHeight { get; set; }

    // 创建地图块模板
    public MapTile MapTile;
    public MapAvatar MapAvatar;

    // 地图显示元素根
    public Transform MapRoot;

    public BattleRoomClient Room { get; private set; }
    public BattleMap Map { get { return Room == null ? null : Room.Battle.Map; } }

    private void Awake()
    {
        SetupGroundOps(); // 所有操作转交当前操作层逻辑
    }

    public void Clear()
    {
        ClearMap();
    }

    // 创建场景显示对象
    public void BuildBattleStage(BattleRoomClient room)
    {
        ClearMap();

        Room = room;

        BuildMapGrids();
        BuildMapItems();
        BuildWarroirs();
        SetupAniPlayer(); // 挂接地图动画播放事件

        MapGround.Area = new Rect(MapRoot.transform.localPosition.x, MapRoot.transform.localPosition.y, Map.Width, Map.Height);
    }

    void ClearMap()
    {
        Tiles = null;
        Avatars = null;

        while (MapRoot.childCount > 0)
            DestroyImmediate(MapRoot.GetChild(0).gameObject);
    }

    // 构建地块层
    StageOpsLayer currentOpLayer = null;
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
    void BuildMapItems()
    {

    }

    // 构建地图上的角色
    public MapAvatar[,] Avatars { get; private set; }
    void BuildWarroirs()
    {
        Avatars = new MapAvatar[Map.Width, Map.Height];
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var warrior = Map.Warriors[x, y];
            if (warrior == null)
                return;

            var avatar = Instantiate(MapAvatar);
            avatar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/Soldier");
            avatar.Warrior = warrior;
            avatar.BattleStage = this;
            avatar.transform.SetParent(MapRoot);
            var sp = avatar.SpriteRender;
            sp.sortingOrder = 2;
            sp.flipX = warrior.Owner != Room.PlayerMe;

            avatar.gameObject.SetActive(true);
            avatar.RefreshAttrs();

            SetAvatarPosition(avatar, x, y);
        });
    }

    void ForeachAvatar(Action<int, int, MapAvatar> act, Func<bool> continueCondition = null)
    {
        FC.For2(Map.Width, Map.Height, (x, y) =>
        {
            var avatar = Avatars[x, y];
            if (avatar == null)
                return;

            act(x, y, avatar);
        }, continueCondition);
    }

    // 设置角色位置
    void SetAvatarPosition(MapAvatar avatar, int x, int y)
    {
        if (avatar != null)
        {
            avatar.X = x;
            avatar.Y = y;
        }

        Avatars[x, y] = avatar;
    }

    // 开始战斗准备阶段
    public void StartPreparing()
    {
        currentOpLayer = new PreparingOps(this);
    }

    // 开始战斗阶段
    public void StartFighting()
    {
        currentOpLayer = new InBattleOps(this);
    }

    // 挂接地图操作逻辑
    void SetupGroundOps()
    {
        MapGround.OnClicked += (float x, float y) => currentOpLayer.OnClicked((int)x, (int)y);
        MapGround.OnStartDragging += (float x, float y) => currentOpLayer.OnStartDragging(x, y);
        MapGround.OnDragging += (float fx, float fy, float cx, float cy) => currentOpLayer.OnDragging(fx, fy, cx, cy);
        MapGround.OnEndDragging += (float fx, float fy, float cx, float cy) => currentOpLayer.OnEndDragging(fx, fy, cx, cy);
    }

    #region 动画播放衔接

    public MapAniPlayer AniPlayer
    {
        get
        {
            if (aniPlayer == null)
                aniPlayer = GetComponent<MapAniPlayer>();

            return aniPlayer;
        }
    } MapAniPlayer aniPlayer;

    MapAvatar GetAvatarByWarrior(Warrior warrior)
    {
        warrior.GetPosInMap(out int x, out int y);
        var avatar = Avatars[x, y];
        Debug.Assert(avatar != null, "warrior should have a avatar in battle map");
        return avatar;
    }

    // 挂接动画播放事件
    void SetupAniPlayer()
    {
        // 角色位置变化
        Room.Battle.OnWarriorPositionExchanged += (int fromX, int fromY, int toX, int toY) =>
        {
            AniPlayer.AddOp(() =>
            {
                var avFrom = Avatars[fromX, fromY];
                var avTo = Avatars[toX, toY];

                SetAvatarPosition(avFrom, toX, toY);
                SetAvatarPosition(avTo, fromX, fromY);
            });
        };

        // 回合开始
        Room.Battle.OnNextRoundStarted += (int player) =>
        {
            if (player != Room.PlayerMe)
                return;

            AniPlayer.AddOp(() => ForeachAvatar((x, y, avatar) => avatar.RefreshAttrs()));
        };

        // 角色攻击
        Room.Battle.OnWarriorAttack += (Warrior attacker, Warrior target) =>
        {
            var avatar = GetAvatarByWarrior(attacker);
            var targetAvatar = GetAvatarByWarrior(target);
            AniPlayer.Add(AniPlayer.MakeAttacking(avatar, targetAvatar));
        };

        // 角色移动
        Room.Battle.OnWarriorMovingOnPath += (Warrior warrior, List<int> path) =>
        {
            var avatar = Avatars[path[0], path[1]];
            Debug.Assert(avatar != null && avatar.Warrior == warrior, "moving target conflicted");

            AniPlayer.Add(AniPlayer.MakeMovingOnPath(avatar.transform, 5, FC.ToArray(path, (i, p, doSkip) => i % 2 == 0 ? p + avatar.CenterOffset.x : p + avatar.CenterOffset.y)));
            Avatars[path[path.Count - 2], path[path.Count - 1]] = avatar;
        };

        // 回合结束
        Room.Battle.OnActionDone += (int player) =>
        {
            if (player != Room.PlayerMe)
                return;

            AniPlayer.AddOp(() => ForeachAvatar((x, y, avatar) =>
            {
                avatar.RefreshAttrs();
                avatar.Color = MapAvatar.ColorDefault;
            }));
        };

        // 角色死亡
        Room.Battle.OnWarriorDying += (Warrior warrior) =>
        {
            var avatar = GetAvatarByWarrior(warrior);
            AniPlayer.Add(AniPlayer.MakeDying(avatar), () => Avatars[avatar.X, avatar.Y] = null);
        };
    }

    #endregion
}
