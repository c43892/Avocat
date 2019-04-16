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
        public Battle Battle { get { return Map?.Battle; } }
        public int IDInMap { get; private set; }
        public bool IsObstacle { get; set; } // 是否是障碍，占据地块

        public BattleMapItem(BattleMap map)
        {
            Map = map;
            IDInMap = map.ItemIDInMap;
            IsObstacle = true; // 默认都占据地块
        }

        // 获取对象在地图中位置
        public virtual void GetPosInMap(out int x, out int y)
        {
            Map.FindXY(this, out x, out y);
        }

        public List<Buff> Buffs { get; } = new List<Buff>();
    }
}
