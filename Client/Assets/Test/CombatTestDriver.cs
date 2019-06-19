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
    public void StartGame()
    {
        var map = new BattleMap(10, 6, (x, y) => TileType.Grass); // test map
        var s = DateTime.Now.ToLocalTime().ToString();
        var bt = new BattlePVE(map, 0, new PlayerInfo { ID = "tester:"+ s, Name = "战斗测试:"+ s }); // test battle

        // 载入地图
        MapReader.ReloadMapInfo();
        //   MapReader.onSetWarrior = (x, y, warrior) => { bt.AddWarriorAt(x, y, warrior);};
        // npcs
        //bt.AddWarriorAt(5, 1, Configuration.Config(new Boar(map) { Team = 2 }));
        //bt.AddWarriorAt(5, 3, Configuration.Config(new Boar(map) { Team = 2 }));
        //bt.AddWarriorAt(5, 5, Configuration.Config(new Boar(map) { Team = 2 }));

        //// heros
        //bt.AddWarriorAt(2, 1, Configuration.Config(new DaiLiWan(bt) { Team = 1 }));
        //bt.AddWarriorAt(2, 2, Configuration.Config(new LuoLiSi(bt) { Team = 1 }));
        //bt.AddWarriorAt(2, 3, Configuration.Config(new YouYinChuan(bt) { Team = 1 }));
        //bt.AddWarriorAt(2, 4, Configuration.Config(new BaLuoKe(bt) { Team = 1 }));
        var data1 = MapReader.GetRandomPlaceForEnemy();
        bt.AddWarriorAt(data1.X, data1.Y, Configuration.Config(new Boar(map) { Team = 2 }));
        var data2 = MapReader.GetRandomPlaceForEnemy();
        bt.AddWarriorAt(data2.X, data2.Y, Configuration.Config(new Boar(map) { Team = 2 }));
        var data3 = MapReader.GetRandomPlaceForEnemy();
        bt.AddWarriorAt(data3.X, data3.Y, Configuration.Config(new Boar(map) { Team = 2 }));

        // heros
        var data4 = MapReader.GetRandomPlaceForChamp();
        bt.AddWarriorAt(data4.X, data4.Y, Configuration.Config(new DaiLiWan(bt) { Team = 1 }));
        var data5 = MapReader.GetRandomPlaceForChamp();
        bt.AddWarriorAt(data5.X, data5.Y, Configuration.Config(new LuoLiSi(bt) { Team = 1 }));
        var data6 = MapReader.GetRandomPlaceForChamp();
        bt.AddWarriorAt(data6.X, data6.Y, Configuration.Config(new YouYinChuan(bt) { Team = 1 }));
        var data7 = MapReader.GetRandomPlaceForChamp();
        bt.AddWarriorAt(data7.X, data7.Y, Configuration.Config(new BaLuoKe(bt) { Team = 1 }));

        // items
      //  bt.AddItemAt(7, 2, Configuration.Config(new Trunk(map)));
      //  bt.AddItemAt(7, 4, Configuration.Config(new Rock(map)));

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
        StartGame();
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
