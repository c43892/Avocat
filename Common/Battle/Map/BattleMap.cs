using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗地图
    /// </summary>
    public class BattleMap
    {
        public Battle Battle { get; set; }

        // 用来给加入地图的 item 进行 id 分配
        public int ItemIDInMap
        {
            get
            {
                return ++itemInMapIndex;
            }
        } int itemInMapIndex = 0;

        public BattleMap(int w, int h)
        {
            Width = w;
            Height = h;
            grids = new int[w, h];
            items = new BattleMapItem[w, h];
            warriors = new Warrior[w, h];
        }

        // 地图尺寸
        public int Width { get; private set; }
        public int Height { get; private set; }

        // 所有地块底图
        public int[, ] Grids
        {
            get
            {
                return grids;
            }
        } int[,] grids;

        // 所有道具
        public BattleMapItem[,] Items
        {
            get
            {
                return items;
            }
        } BattleMapItem[,] items;

        // 所有角色
        Warrior[,] warriors;

        public Warrior GetWarriorAt(int x, int y)
        {
            return warriors[x, y];
        }

        public void SetWarriorAt(int x, int y, Warrior warrior)
        {
            Debug.Assert(warrior == null || warrior.Map == this, "the warrior is not in the map");
            warriors[x, y] = warrior;
        }

        public bool FindXY(Warrior warrior, out int px, out int py)
        {
            var tx = 0;
            var ty = 0;
            var found = false;

            FC.For2(Width, Height, (x, y) =>
            {
                if (warriors[x, y] == warrior)
                {
                    tx = x;
                    ty = y;
                    found = true;
                }
            }, () => !found);

            px = tx;
            py = ty;

            return found;
        }

        // 迭代所有非空战斗角色
        public void ForeachWarriors(Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var warrior = warriors[x, y];
                if (warrior != null)
                    act(x, y, warrior);

            }, continueCondition);
        }

        // 根据 id 寻找指定角色
        public Warrior GetWarriorsByID(int idInMap)
        {
            Warrior targetWarrior = null;
            ForeachWarriors((x, y, warrior) =>
            {
                if (warrior.IDInMap == idInMap)
                    targetWarrior = warrior;
            }, () => targetWarrior == null);

            return targetWarrior;
        }
    }
}
