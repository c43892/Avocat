using System;
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

    public void Start()
    {
        msgLooper = new BattleMessageLooper();

        // setup the local replay system
        Recoder.LoadAll();
        msgLooper.OnMessageIn += (byte[] data) =>
        {
            if (currentReplay != null)
                currentReplay.Messages.Add(data);
        };

        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);

        StartCoroutine(msgLooper.Loop());
    }

    // 开始新游戏
    public void OnStartNewBattle()
    {
        // test map
        var map = new BattleMap(10, 6);
        map.SetWarriorAt(2, 2, new Warrior(map, 2) { Owner = 1, AttackRange = 5, Power = 1 });
        map.SetWarriorAt(2, 3, new Warrior(map, 2) { Owner = 1, AttackRange = 5, Power = 2 });
        map.SetWarriorAt(2, 4, new Warrior(map, 2) { Owner = 1, AttackRange = 5, Power = 3 });

        // npcs
        var npc0 = new Warrior(map, 5) { Owner = 2, AttackRange = 1, Power = 1, MoveRange = 2 };
        var npc1 = new Warrior(map, 5) { Owner = 2, AttackRange = 1, Power = 2, MoveRange = 2 };
        var npc2 = new Warrior(map, 5) { Owner = 2, AttackRange = 1, Power = 3, MoveRange = 2 };
        map.SetWarriorAt(5, 1, npc0);
        map.SetWarriorAt(5, 3, npc1);
        map.SetWarriorAt(5, 5, npc2);

        // test battle
        var bt = new BattlePVE(map, 0, new PlayerInfo { ID = "tester", Name = "战斗测试" }, npc0, npc1, npc2);
        // test room
        var room = new BattleRoomClient(new BattlePVERoom(bt)) { PlayerMe = 1 };

        // setup the fake message loop
        room.BMS = msgLooper;
        msgLooper.Clear();
        room.RegisterBattleMessageHandlers(msgLooper);

        BattleStage.BuildBattleStage(room);

        room.Battle.OnPlayerPrepared.Add((int player) =>
        {
            if (room.Battle.AllPrepared)
            {
                PreparingUI.SetActive(false);
                BattleStageUI.gameObject.SetActive(true);
                BattleStage.StartFighting();
            }
        });

        room.Battle.OnBattleEnded.Add((winner) =>
        {
            if (currentReplay != null)
            {
                Recoder.AddReplay(currentReplay);
                Recoder.SaveAll();
            }

            GameOverUI.SetActive(true);
            GameOverUI.transform.Find("Title").GetComponent<Text>().text = winner == room.PlayerMe ? "Win" : "Lose";
            BattleStageUI.gameObject.SetActive(false);
        });

        room.Battle.OnNextRoundStarted.Add((player) =>
        {
            if (player != Room.PlayerMe)
                return;

            var availableCards = new List<BattleCard>();
            availableCards.AddRange(bt.AvailableCards);
            BattleStageUI.RefreshCardsAvailable(availableCards);
        });

        room.Battle.OnWarriorMovingOnPath.Add((warrior, x, y, path) =>
        {
            var availableCards = new List<BattleCard>();
            availableCards.AddRange(bt.AvailableCards);
            BattleStageUI.RefreshCardsAvailable(availableCards);
        });

        (room.Battle as BattlePVE).OnBattleCardsExchange.Add((int g1, int n1, int g2, int n2) =>
        {
            BattleStageUI.RefreshCardsAvailable(bt.AvailableCards);
            BattleStageUI.RefreshCardsStarshed(bt.StashedCards);
        });

        BattleStage.gameObject.SetActive(true);
        BattleStage.StartPreparing();
        StartingUI.SetActive(false);
        PreparingUI.SetActive(true);
        currentReplay = this.Recoder.InReplaying ? null : new BattleReplay { Time = DateTime.Now.Ticks };
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
