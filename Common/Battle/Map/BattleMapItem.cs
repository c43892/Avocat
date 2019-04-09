using System;
using System.Collections.Generic;
using System.Text;
using Swift;
using System.Diagnostics;

namespace Avocat
{
    /// <summary>
    /// 道具
    /// </summary>
    public class BattleMapItem
    {
        public BattleMap Map { get; set; }

        public BattleMapItem(BattleMap map)
        {
            Map = map;
        }

        // 获取对象在地图中位置
        public virtual bool GetPosInMap(out int x, out int y)
        {
            return FindItemsInMap(Map.Items, out x, out y);
        }

        // 获取对象在地图中位置
        protected bool FindItemsInMap(BattleMapItem[,] items, out int x, out int y)
        {
            Debug.Assert(Map != null, "Warrior is not in a map");
            int fx = -1;
            int fy = -1;
            bool found = false;
            FC.For2(Map.Width, Map.Height, (int px, int py) =>
             {
                 if (items[px, py] == this)
                 {
                     fx = px;
                     fy = py;
                     found = true;
                 }
             }, () => !found);

            x = fx;
            y = fy;
            return found;
        }
    }
}
