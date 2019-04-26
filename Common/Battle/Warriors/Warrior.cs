using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Swift;

namespace Avocat
{
    public interface ITransformable
    {
        string State { set; }
    }

    /// <summary>
    /// 战斗角色
    /// </summary>
    public class Warrior : BattleMapItem
    {
        public Warrior(BattleMap map)
            :base (map)
        {
        }

        public WarriorAI AI { get; set; }

        public int AvatarID { get; private set; } // 具体的角色形象 ID
        public int Team { get; set; } // 是属于哪一个玩家

        public int MaxHP { get; set; } // 最大血量
        public int HP { get; set; } // 血量

        public string AttackingType { get; set; } // 攻击类型：physic - 物理，magic - 法术，chaos - 混乱

        public int ATK { get; set; } // 物攻
        public int ATKInc { get; set; } // 物攻加成
        public int ATKMore { get; set; } // 物攻加成

        public int POW { get; set; } // 法攻
        public int POWInc { get; set; } // 法攻加成
        public int POWMore { get; set; } // 法攻加成

        // 基本攻击力
        public int BasicAttackValue
        {
            get
            {
                if (AttackingType == "physic")
                    return ATK + POW / 2;
                else if (AttackingType == "magic")
                    return ATK / 2 + POW;
                else if (AttackingType == "chaos")
                    return ATK /2 + POW / 2;

                Debug.Assert(false, "unknown attacking type: " + AttackingType);
                return 0;
            }
        }

        public int ARM { get; set; } // 物盾
        public int RES { get; set; } // 魔盾

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
            skill.Warrior = this;
            activeSkills[name] = skill;

            if (asDefaultActiveSkill)
                defaultSkillName = name;
        }

        // 获取主动技能
        public ActiveSkill GetActiveSkillByName(string name)
        {
            Debug.Assert(activeSkills.ContainsKey(name), "skill named: " + name + " doest not exist.");
            return activeSkills[name];
        }

        // 获取默认主动技能
        public ActiveSkill GetDefaultActiveSkill()
        {
            return defaultSkillName == null ? null : GetActiveSkillByName(defaultSkillName);
        }

        // 移除主动技能
        public void RemoveActiveSkill(ActiveSkill skill)
        {
            Debug.Assert(skill.Warrior == this, "skill named: " + skill.Name + " doest not exist.");
            RemoveActiveSkill(skill.Name);
        }

        // 移除主动技能
        public void RemoveActiveSkill(string name)
        {
            Debug.Assert(activeSkills.ContainsKey(name) && activeSkills[name].Warrior == this, "skill named: " + name + " doest not exist.");
            activeSkills.Remove(name);
            if (defaultSkillName == name)
                defaultSkillName = null;
        }
    }
}