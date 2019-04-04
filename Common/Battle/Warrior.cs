using System;
using System.Collections.Generic;
using System.Text;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗人员
    /// </summary>
    public class Warrior : BattleMapItem
    {
        public Warrior(BattleMap map)
            :base (map)
        {
        }

        public int AvatarID { get; private set; }
        public bool IsOpponent { get; set; }

        // 获取对象在地图中位置
        public override bool GetPosInMap(out int x, out int y)
        {
            return FindItemsInMap(Map.Warriors, out x, out y);
        }
    }
}