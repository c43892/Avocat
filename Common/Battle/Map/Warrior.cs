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
        public Warrior(BattleMap map, int maxHp)
            :base (map)
        {
            MaxHp = maxHp;
            Hp = MaxHp;
        }

        public int AvatarID { get; private set; } // 具体的角色形象 ID
        public int Owner { get; set; } // 是属于哪一个玩家
        public int Hp { get; set; } // 血量
        public int MaxHp { get; set; } // 最大血量
        public int Power { get; set; } // 攻击力
        public int Shield { get; set; } // 护甲
        public int AttackRange { get; set; } // 最大攻击距离
        public int MoveRange { get; set; } // 最大移动距离

        public bool InAttackRange(int tx, int ty)
        {
            GetPosInMap(out int x, out int y);
            return MU.ManhattanDist(x, y, tx, ty) <= AttackRange;
        }

        // 本回合是否已经移动过
        public bool Moved {
            get
            {
                return moved || ActionDone;
            }
            set
            {
                moved = value;
            }
        } bool moved = false; 
        public bool ActionDone { get; set; } // 角色在本回合的行动已经结束

        public bool IsDead { get { return Hp <= 0; } }

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
            return Map.FindXY(this, out x, out y);
        }
    }
}