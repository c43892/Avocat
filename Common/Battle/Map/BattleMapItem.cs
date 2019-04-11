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
        public BattleMap Map { get; private set; }
        public int IDInMap { get; private set; }

        public BattleMapItem(BattleMap map)
        {
            Map = map;
            IDInMap = map.ItemIDInMap;
        }

        // 获取对象在地图中位置
        public virtual void GetPosInMap(out int x, out int y)
        {
            Map.FindXY(this, out x, out y);
        }
    }
}
