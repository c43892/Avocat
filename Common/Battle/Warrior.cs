using System;
using System.Collections.Generic;
using System.Text;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗角色
    /// </summary>
    public class Warrior : BattleMapItem
    {
        public Warrior(BattleMap map)
            :base (map)
        {
        }

        public int AvatarID { get; private set; } // 具体的角色 ID
        public bool IsOpponent { get; set; } // 是否是对手

        public int Power { get; set; } // 攻击力

        // 角色要移动的路径信息放在角色身上
        public List<int> MovingPath
        {
            get
            {
                return movingPath;
            }
        } List<int> movingPath = new List<int>();

        // 获取对象在地图中位置
        public override bool GetPosInMap(out int x, out int y)
        {
            return FindItemsInMap(Map.Warriors, out x, out y);
        }
    }
}