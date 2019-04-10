using System;
using System.Collections.Generic;
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
        public Warrior[,] Warriors
        {
            get
            {
                return warriors;
            }
        } Warrior[,] warriors;

        // 迭代所有非空战斗角色
        public void ForeachWarriors(Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var warrior = Warriors[x, y];
                if (warrior != null)
                    act(x, y, warrior);

            }, continueCondition);
        }
    }
}
