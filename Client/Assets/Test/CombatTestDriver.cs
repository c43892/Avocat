﻿using System;
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
        var map = new BattleMap(10, 6); // test map
        var bt = new BattlePVE(map, 0, new PlayerInfo { ID = "tester", Name = "战斗测试" }); // test battle

        // npcs
        FC.Async2Sync(bt.AddWarriorAt(5, 1, Configuration.Config(new Boar(map) { Team = 2 })));
        FC.Async2Sync(bt.AddWarriorAt(5, 3, Configuration.Config(new Boar(map) { Team = 2 })));
        FC.Async2Sync(bt.AddWarriorAt(5, 5, Configuration.Config(new Boar(map) { Team = 2 })));

        // heros
        FC.Async2Sync(bt.AddWarriorAt(2, 1, Configuration.Config(new DaiLiWan(bt) { Team = 1 })));
        FC.Async2Sync(bt.AddWarriorAt(2, 2, Configuration.Config(new LuoLiSi(bt) { Team = 1 })));
        FC.Async2Sync(bt.AddWarriorAt(2, 3, Configuration.Config(new YouYinChuan(bt) { Team = 1 })));
        FC.Async2Sync(bt.AddWarriorAt(2, 4, Configuration.Config(new BaLuoKe(bt) { Team = 1 })));

        // items
        FC.Async2Sync(bt.AddItemAt(7, 2, Configuration.Config(new Trunk(map))));
        FC.Async2Sync(bt.AddItemAt(7, 4, Configuration.Config(new Rock(map))));

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
            StartGame();
            PlayReplay(replay);
        });

        // link the logic event to the stage and ui logic
        BattleStage.SetupEventHandler(room);
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
