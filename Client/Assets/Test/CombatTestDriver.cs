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

    BattleRoomClient Room { get { return BattleStage.Room; } }

    BattleMessageLooper msgLooper;
    LocalBattleRecorder Recoder
    {
        get
        {
            return GetComponent<LocalBattleRecorder>();
        }
    }

    BattleReplay currentReplay;

    public new void Start()
    {
        base.Start();

        msgLooper = new BattleMessageLooper();

        // setup the local replay system
        Recoder.LoadAll();
        msgLooper.OnMessageIn += (byte[] data) =>
        {
            if (currentReplay == null)
            {
                currentReplay = new BattleReplay { Time = DateTime.Now.Ticks };
                Recoder.AddReplay(currentReplay);
            }

            currentReplay.Messages.Add(data);
            Recoder.SaveAll();
        };

        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);

        // StartCoroutine(msgLooper.Loop());
        GameCore.Instance.Get<CoroutineManager>().Start(msgLooper.Loop());
    }

    // 开始新游戏
    public void OnStartNewBattle()
    {
        var map = new BattleMap(10, 6); // test map
        var bt = new BattlePVE(map, 0, new PlayerInfo { ID = "tester", Name = "战斗测试" }); // test battle

        // heros
        FC.Async2Sync(bt.AddWarriorAt(2, 1, new DaiLiWan(bt) { Team = 1 }));
        FC.Async2Sync(bt.AddWarriorAt(2, 2, new LuoLiSi(bt) { Team = 1 }));
        FC.Async2Sync(bt.AddWarriorAt(2, 3, new YouYinChuan(bt) { Team = 1 }));
        FC.Async2Sync(bt.AddWarriorAt(2, 4, new BaLuoKe(bt) { Team = 1 }));

        // npcs
        FC.Async2Sync(bt.AddWarriorAt(5, 1, new Boar(map) { Team = 2, AttackRange = 1, ATK = 1 }));
        FC.Async2Sync(bt.AddWarriorAt(5, 3, new Boar(map) { Team = 2, AttackRange = 1, ATK = 1 }));
        FC.Async2Sync(bt.AddWarriorAt(5, 5, new Boar(map) { Team = 2, AttackRange = 1, ATK = 1 }));

        // items
        FC.Async2Sync(bt.AddItemAt(7, 2, new Trunk(map)));
        FC.Async2Sync(bt.AddItemAt(7, 4, new Rock(map)));

        // test room
        var room = new BattleRoomClient(new BattlePVERoom(bt)) { PlayerMe = 1 };

        // setup the fake message loop
        room.BMS = msgLooper;
        msgLooper.Clear();
        room.RegisterBattleMessageHandlers(msgLooper);

        // build up the whole scene
        BattleStage.Build(room);

        // link the logic event to the stage and ui logic
        BattleStage.SetupEventHandler(room);
        BattleStageUI.SetupEventHandler(room);
        this.SetupEventHandler(room);

        StartingUI.SetActive(false);
        PreparingUI.SetActive(true);
        BattleStage.gameObject.SetActive(true);
        BattleStage.StartPreparing();

        currentReplay = null;
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
        OnStartNewBattle();
        currentReplay = Recoder.Replays[i];
        Recoder.Play(currentReplay, (data) => msgLooper.SendRaw(data));
        currentReplay.Messages.Clear();
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
