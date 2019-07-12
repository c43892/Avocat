using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public enum EquipType
    {
        Weapon,
        Armor,
        Potion,
        Trinket
    }

    /// <summary>
    /// 装备对象
    /// </summary>
    public class Equip
    {
        public EquipType EquipType { get; set; }

        // 所属队伍
        public int Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                foreach (var attr in Attrs)
                    attr.Team = team;
            }
        } int team;

        // 基本属性
        public int MaxHP { get; set; } // 加血
        public int MaxES { get; set; } // 加盾最大值
        public int ATK { get; set; } // 加物攻
        public int POW { get; set; } // 加法攻
        public int ARM { get; set; } // 物盾
        public int RES { get; set; } // 法盾

        // 装备特殊属性
        readonly List<EquipAttr> Attrs = new List<EquipAttr>();
        public void AddAttr(EquipAttr attr)
        {
            attr.Team = team;
            Attrs.Add(attr);
        }

        // 战斗准备，添加各种 buff 之类的东西
        public void OnPreparingBattle(Battle bt)
        {
            // 基本属性生效
            bt.Map.ForeachObjs<Warrior>((x, y, warrior) =>
            {
                var hero = warrior as Hero;
                hero.MaxHP += MaxHP;
                hero.MaxES += MaxES;
                hero.ATK += ATK;
                hero.POW += POW;
                hero.ARM += ARM;
                hero.RES += RES;
            }, (warrior) => warrior is Hero && warrior.Team == Team);

            // 特殊属性生效
            foreach (var attr in Attrs)
                attr.OnPreparingBattle(bt);
        }
    }
}
