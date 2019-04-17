using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗角色
    /// </summary>
    public class Warrior : BattleMapItem
    {
        public Warrior(BattleMap map, int maxHP, int maxES)
            :base (map)
        {
            MaxHP = maxHP;
            MaxES = maxES;
            HP = MaxHP;
            ES = 0;
        }

        public int AvatarID { get; private set; } // 具体的角色形象 ID

        public int Owner { get; set; } // 是属于哪一个玩家

        public int MaxHP { get; set; } // 最大血量
        public int HP { get; set; } // 血量

        public int ATK { get; set; } // 攻击力

        public int MaxES { get; set; } // 最大护盾
        public int ES { get; set; } // 护盾

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

        public bool IsDead { get { return HP <= 0; } }

        // 角色要移动的路径信息放在角色身上
        public List<int> MovingPath { get; } = new List<int>();

        // 获取对象在地图中位置
        public override void GetPosInMap(out int x, out int y)
        {
            Debug.Assert(Map != null, "warrior is not in map now");
            Map.FindXY(this, out x, out y);
        }

        // 所有主动技能
        string defaultSkillName = null;
        Dictionary<string, ActiveSkill> activeSkills = new Dictionary<string, ActiveSkill>();

        // 添加主动技能
        public void AddActiveSkill(ActiveSkill skill, bool asDefaultActiveSkill = true)
        {
            var name = skill.Name;
            Debug.Assert(!activeSkills.ContainsKey(name), "skill named: " + name + " has aleardy existed.");
            skill.Target = this;
            activeSkills[name] = skill;

            if (asDefaultActiveSkill)
                defaultSkillName = name;
        }

        // 获取主动技能
        public ActiveSkill GetActiveSkillByName(string name)
        {
            Debug.Assert(!activeSkills.ContainsKey(name), "skill named: " + name + " doest not exist.");
            var skill = activeSkills[name];
            skill.Target = null;
            return skill;
        }

        // 获取默认主动技能
        public ActiveSkill GetDefaultActiveSkill()
        {
            Debug.Assert(defaultSkillName != null, "no default active skill");
            return GetActiveSkillByName(defaultSkillName);
        }
    }
}