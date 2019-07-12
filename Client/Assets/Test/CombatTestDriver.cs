using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class CombatTestDriver : GameDriver
{
    public BattleStage BattleStage;
    public GameObject PreparingUI;
    public GameObject StartingUI;
    public GameObject GameOverUI;
    public BattleStageUI BattleStageUI;
    public MapReader MapReader;

    BattleRoomClient Room { get { return BattleStage.Room; } }
    Battle Battle { get { return Room?.Battle; } }

    BattleMessageLooper msgLooper;
    LocalBattleRecorder Recoder
    {
        get
        {
            return GetComponent<LocalBattleRecorder>();
        }
    }

    public new void Start()
    {
        base.Start();

        msgLooper = new BattleMessageLooper();

        // setup the local replay system
        Recoder.LoadAll();
        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);

        // enable multiTouch
        Input.multiTouchEnabled = true;

        GameCore.Instance.Get<CoroutineManager>().Start(msgLooper.Loop());
    }

    // 开始新游戏
    public void OnStartNewBattle()
    {
        StartGame();
        Recoder.AddReplay(Battle.Replay);
    }

    // 开始游戏，可能是新游戏，也可能是播放录像
    public void StartGame(BattleReplay battleReplay = null)
    {
        var map = new BattleMap(10, 6, (x, y) => TileType.Grass); // test map
        var s = DateTime.Now.ToLocalTime().ToString();
        var bt = new BattlePVE(map, 0, new PlayerInfo { ID = "tester:"+ s, Name = "战斗测试:"+ s }); // test battle

        // 载入地图
        if (battleReplay == null)
        {
            map.MapInfo = MapReader.ReloadMapInfo();
            bt.Replay.MapName = MapReader.MapNameList[MapReader.ArrayIndex];
        }
        else
        {
            MapReader.ReloadMapByName(battleReplay.MapName);
            map.MapInfo = MapReader.Map;
        }
        
        map.GetRespawnPlace();

        // 配置buff显示列表
        BattleStageUI.CharacterInfoUI.BuffDisplayList = Configuration.Config(new BuffToDisplay());

        // npcs
        FC.For(3, (i) =>
        {
            bt.AddWarriorAt(map.RespawnForEnemy[i].X, map.RespawnForEnemy[i].Y, Configuration.Config(new Boar(map) { Team = 2 }));
        });

        // heros

        // 黛丽万
        var dlw = Configuration.Config(new DaiLiWan(bt) { Team = 1 });
        dlw.AddRune(new DaiLiWanRune1());
        dlw.AddRune(new DaiLiWanRune2());
        dlw.AddRune(new DaiLiWanRune3());
        dlw.AddRune(new DaiLiWanRune4());
        dlw.RunAllRune2PrepareBattle();
        bt.AddWarriorAt(map.RespawnForChamp[0].X, map.RespawnForChamp[0].Y, dlw);

        // 洛里斯
        var lls = Configuration.Config(new LuoLiSi(bt) { Team = 1 });
        lls.AddRune(new LuoLiSiRune1());
        lls.AddRune(new LuoLiSiRune2());
        lls.AddRune(new LuoLiSiRune3());
        lls.AddRune(new LuoLiSiRune4());
        lls.RunAllRune2PrepareBattle();
        bt.AddWarriorAt(map.RespawnForChamp[1].X,map.RespawnForChamp[1].Y, lls);

        // 巴洛克
        var blk = Configuration.Config(new BaLuoKe(bt) { Team = 1 });
        blk.AddRune(new BaLuoKeRune1());
        blk.AddRune(new BaLuoKeRune2());
        blk.AddRune(new BaLuoKeRune3());
        blk.AddRune(new BaLuoKeRune4());
        blk.RunAllRune2PrepareBattle();
        bt.AddWarriorAt(map.RespawnForChamp[2].X, map.RespawnForChamp[2].Y, blk);

        // 游川隐
        var ycy = Configuration.Config(new YouChuanYin(bt) { Team = 1 });
        ycy.AddRune(new YouChuanYinRune1());
        ycy.AddRune(new YouChuanYinRune2());
        ycy.AddRune(new YouChuanYinRune3());
        ycy.AddRune(new YouChuanYinRune4());
        ycy.RunAllRune2PrepareBattle();
        bt.AddWarriorAt(map.RespawnForChamp[3].X, map.RespawnForChamp[3].Y, ycy);

        // items
        // bt.AddItemAt(7, 2, Configuration.Config(new Trunk(map)));
        // bt.AddItemAt(7, 4, Configuration.Config(new Rock(map)));

        // test room
        var room = new BattleRoomClient(new BattlePVERoom(bt)) { PlayerMe = 1 };
        room.BattleRoom.ReplayChanged += () =>
        {
            if (!Recoder.Exists(bt.Replay))
                Recoder.AddReplay(bt.Replay);

            Recoder.SaveAll();
        };

        // setup the fake message loop
        room.BMS = msgLooper;
        msgLooper.Clear();
        room.RegisterBattleMessageHandlers(msgLooper);

        // build up the whole scene
        BattleStage.Build(room, (replay) =>
        {
            // var aniPlayer = BattleStage.GetComponent<MapAniPlayer>();
            // aniPlayer.AnimationTimeScaleFactor = 10000;
            StartGame();
            PlayReplay(replay);
        });

        // link the logic event to the stage and ui logic
        BattleStage.SetupEventHandler(room);
        BattleStage.SetupUIHandler(room);
        BattleStageUI.SetupEventHandler(room);
        this.SetupEventHandler(room);

        StartingUI.SetActive(false);
        PreparingUI.SetActive(true);
        BattleStage.gameObject.SetActive(true);
        BattleStage.StartPreparing();
    }

    // 准备完毕
    public void OnPreparingDown()
    {
        Room.DoPrepared();
    }

    // 显示录像列表
    public void OnShowReplays()
    {
        StartingUI.transform.Find("Start").gameObject.SetActive(false);
        StartingUI.transform.Find("Replays").gameObject.SetActive(true);
        StartingUI.transform.Find("PlayReplays").gameObject.SetActive(false);

        var replays = Recoder.Replays;
        for (var i = 0; i < 5; i++)
        {
            var btn = StartingUI.transform.Find("Replays").Find("Replay" + i).gameObject;
            btn.SetActive(i < replays.Count);
            var txt = btn.GetComponentInChildren<Text>();

            if (i < replays.Count)
                txt.text = (new DateTime(replays[i].Time)).ToShortTimeString();
        }
    }

    // 播放游戏录像
    public void OnPlayReplay(int i)
    {
        StartGame(Recoder.Replays[i]);
        PlayReplay(Recoder.Replays[i]);
    }

    public void PlayReplay(BattleReplay replay)
    {
        Battle.Replay.Messages.AddRange(replay.Messages);
        Recoder.Play(replay, (data) => msgLooper.SendRaw(data));
    }

    // 游戏结束确定
    public void OnGameOverOk()
    {
        GameOverUI.SetActive(false);
        BattleStage.Clear();
        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);
        StartingUI.transform.Find("Start").gameObject.SetActive(true);
        StartingUI.transform.Find("Replays").gameObject.SetActive(false);
        StartingUI.transform.Find("PlayReplays").gameObject.SetActive(true);
    }
}
