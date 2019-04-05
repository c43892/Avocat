using System;
using System.Diagnostics;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗对象，包括一场战斗所需全部数据，在初始数据和操作过程完全一致的情况下，可完全复现一场战斗过程。
    /// 但战斗对象本身不包括战斗驱动过程，而是有 BattleRoom 驱动。
    /// </summary>
    public class Battle
    {
        // 由初始随机种子确定的伪随机序列
        SRandom srand;

        // 战斗地图
        public BattleMap Map { get; private set; }

        // 参与战斗的各玩家
        public PlayerInfo[] Players;

        public Battle(int mapWidth, int mapHeight, int randSeed, params PlayerInfo[] players)
        {
            srand = new SRandom(randSeed);
            Map = new BattleMap(mapWidth, mapHeight);
            Players = players;
        }

        // 交换英雄位置
        public void ExchangeWarroirsPosition(int fromX, int fromY, int toX, int toY)
        {
            var item = Map.Warriors[fromX, fromY];
            Map.Warriors[fromX, fromY] = Map.Warriors[toX, toY];
            Map.Warriors[toX, toY] = item;
        }
    }
}