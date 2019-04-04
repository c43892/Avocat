using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

public class CombatTestDriver : MonoBehaviour
{
    public BattleStage BattleStage;
    public MapTile TestMapTile;
    public MapAvatar TestMapWarrior;

    public GameObject PreparingUI;
    public GameObject StartingUI;

    BattleRoomClient Room { get { return BattleStage.Room; } }

    public void Start()
    {
        BattleStage.MapTileCreator = (int tileType) => Instantiate(TestMapTile);
        BattleStage.MapWarriorCreator = (int avatarType) => Instantiate(TestMapWarrior);

        // test battle
        var player = new PlayerInfo();
        player.ID = "tester";
        player.Name = "战斗测试";
        var bt = new Battle(22, 12, 0, player);

        // test map
        var map = bt.Map;
        map.Warriors[2, 2] = new Warrior(map);
        map.Warriors[2, 4] = new Warrior(map);
        map.Warriors[2, 6] = new Warrior(map);

        map.Warriors[19, 3] = new Warrior(map);
        map.Warriors[19, 5] = new Warrior(map);
        map.Warriors[19, 7] = new Warrior(map);
        map.Warriors[19, 3].IsOpponent = true;
        map.Warriors[19, 5].IsOpponent = true;
        map.Warriors[19, 7].IsOpponent = true;

        // test room
        var room = new BattleRoomClient(bt);
        BattleStage.BuildBattleStage(room);

        Room.OnAllPrepared += () =>
        {
            PreparingUI.SetActive(false);
        };

        BattleStage.gameObject.SetActive(false);
        StartingUI.SetActive(true);
    }

    // 开始新游戏
    public void OnStartNewGame()
    {
        BattleStage.gameObject.SetActive(true);
        BattleStage.StartPreparing();
        StartingUI.SetActive(false);
        PreparingUI.SetActive(true);
    }

    // 准备完毕
    public void OnPreparingDown()
    {
        Room.Prepared();
        PreparingUI.SetActive(false);
        BattleStage.StartFighting();
    }
}
