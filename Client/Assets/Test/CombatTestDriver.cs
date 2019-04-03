using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Battle;

public class CombatTestDriver : MonoBehaviour
{
    public BattleStage BattleStage;
    public MapTile TestMapTile;
    public MapWarrior TestMapWarrior;

    public void Start()
    {
        BattleStage.MapTileCreator = (int tileType) => Instantiate(TestMapTile);
        BattleStage.MapWarriorCreator = (int avatarType) => Instantiate(TestMapWarrior);

        // test battle
        var bt = new Battle.Battle(22, 12, 0);

        // test map
        var map = bt.Map;
        map.Warriors[2, 2] = new Warrior();
        map.Warriors[2, 4] = new Warrior();
        map.Warriors[2, 6] = new Warrior();

        map.Warriors[19, 3] = new Warrior();
        map.Warriors[19, 5] = new Warrior();
        map.Warriors[19, 7] = new Warrior();
        map.Warriors[19, 3].IsOpponent = true;
        map.Warriors[19, 5].IsOpponent = true;
        map.Warriors[19, 7].IsOpponent = true;

        // test room
        var room = new BattleRoomClient(bt);
        BattleStage.BuildBattleStage(room);
    }
}
