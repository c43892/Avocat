﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class CombatTestDriver : MonoBehaviour
{
    public BattleStage BattleStage;
    public GameObject PreparingUI;
    public GameObject StartingUI;
    public GameObject GameOverUI;

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

    public void Start()
    {
        msgLooper = new BattleMessageLooper();

        // setup the local replay system
        Recoder.LoadAll();
        msgLooper.OnMessageIn += (byte[] data) =>
        {
            currentReplay.Messages.Add(data);
        };

        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);
    }

    public void Update()
    {
        msgLooper.MoveNext();
    }

    // 开始新游戏
    public void OnStartNewBattle()
    {
        // test battle
        var player = new PlayerInfo
        {
            ID = "tester",
            Name = "战斗测试"
        };

        var bt = new Battle(22, 12, 0, player);

        // test map
        var map = bt.Map;
        map.Warriors[2, 2] = new Warrior(map, 2, 1);
        map.Warriors[2, 4] = new Warrior(map, 2, 1);
        map.Warriors[2, 6] = new Warrior(map, 2, 1);
        map.Warriors[2, 2].Owner = 1;
        map.Warriors[2, 4].Owner = 1;
        map.Warriors[2, 6].Owner = 1;

        map.Warriors[19, 3] = new Warrior(map, 2, 1);
        map.Warriors[19, 5] = new Warrior(map, 2, 1);
        map.Warriors[19, 7] = new Warrior(map, 2, 1);
        map.Warriors[19, 3].Owner = 2;
        map.Warriors[19, 5].Owner = 2;
        map.Warriors[19, 7].Owner = 2;

        // test room
        var room = new BattleRoomClient(bt)
        {
            PlayerMe = 1
        };

        // setup the fake message loop
        room.BMS = msgLooper;
        msgLooper.Clear();
        room.RegisterBattleMessageHandlers(msgLooper);

        BattleStage.BuildBattleStage(room);

        room.OnAllPrepared += () =>
        {
            PreparingUI.SetActive(false);
            BattleStage.StartFighting();
        };

        room.OnBattleEnded += (winner) =>
        {
            BattleStage.AniPlayer.Add(null, () =>
            {
                GameOverUI.SetActive(true);
                GameOverUI.transform.Find("Title").GetComponent<Text>().text = winner == room.PlayerMe ? "Win" : "Lose";
            });

            Recoder.AddReplay(currentReplay);
            Recoder.SaveAll();
        };

        BattleStage.gameObject.SetActive(true);
        BattleStage.StartPreparing();
        StartingUI.SetActive(false);
        PreparingUI.SetActive(true);
        currentReplay = new BattleReplay
        {
            Time = DateTime.Now.Ticks
        };
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
        var replay = Recoder.Replays[i];
        Recoder.Play(replay, (data) =>
        {
            msgLooper.SendRaw(data);
        });
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
