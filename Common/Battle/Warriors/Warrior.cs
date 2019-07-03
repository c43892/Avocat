using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 有变身能力
    /// </summary>
    public interface ITransformable
    {
        string State { set; get; }
    }

    /// <summary>
    /// 战斗角色
    /// </summary>
    public class Warrior : BattleMapObj
    {
        public Warrior(BattleMap map)
            :base (map)
        {
        }

        public WarriorAI AI { get; set; } // AI 类型
        public bool IsBoss { get; set; } = false; // 是否是 boss

        public int AvatarID { get; private set; } // 具体的角色形象 ID
        public int Team { get; set; } // 是属于哪一个玩家

        public int MaxHP { get; set; } // 最大血量
        public int HP { get; set; } // 血量

        public string AttackingType { get; set; } // 攻击类型：physic - 物理，magic - 法术，chaos - 混乱

        public int ATK { get; set; } // 物攻
        public int ATKInc { get; set; } // 物攻加成
        public int ATKMore { get; set; } // 物攻加成
        public int Crit { get; set; } // 暴击系数

        public int POW { get; set; } // 法攻
        public int POWInc { get; set; } // 法攻加成
        public int POWMore { get; set; } // 法攻加成
        public PatternSkill PatternSkill { get => GetPassiveSkill<PatternSkill>(); }
        public bool IsSkillReleased { get; set; }

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

        // 计算防御力评估值
        public int GetEstimatedDefence(string attackType = null)
        {
            if (attackType == "physic")
                return ARM;
            else if (attackType == "magic")
                return RES;
            else
                return (ARM + RES) / 2;
        }

        public int ARM { get; set; } // 物盾
        public int RES { get; set; } // 魔盾

        public int MaxES { get; set; } // 最大护盾
        public int ES { get; set; } // 护盾

        public TileType StandableTiles; // 可以站立的地块类型
        public int[] AttackRange { get; set; } // 最大攻击距离
        public int MoveRange { get; set; } // 最大移动距离

        public bool InAttackRange(int tx, int ty)
        {
            GetPosInMap(out int x, out int y);
            var dist = MU.ManhattanDist(x, y, tx, ty);
            return FC.IndexOf(AttackRange, dist) >= 0;
        }

        public bool Moved { get; set; } // 本回合是否已经移动过
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

        #region 技能相关

        // 所有主动技能
        string defaultSkillName = null;
        Dictionary<string, ActiveSkill> activeSkills = new Dictionary<string, ActiveSkill>();

        // 添加主动技能
        public void AddActiveSkill(ActiveSkill skill, bool asDefaultActiveSkill = true)
        {
            var name = skill.ID;
            Debug.Assert(!activeSkills.ContainsKey(name), "skill named: " + name + " has aleardy existed. Use ReplaceActiveSkill to replace it.");
            activeSkills[name] = skill;

            if (asDefaultActiveSkill)
                defaultSkillName = name;
        }

        // 替换同名主动技能
        public ActiveSkill ReplaceActiveSkill(ActiveSkill skill)
        {
            var s = activeSkills[skill.ID];
            activeSkills.Remove(skill.ID);
            AddActiveSkill(skill, defaultSkillName == skill.ID);
            return s;
        }

        // 获取主动技能
        public ActiveSkill GetActiveSkillByName(string name)
        {
            return activeSkills.ContainsKey(name) ? activeSkills[name] : null;
        }

        // 获取主动技能
        public T GetActiveSkill<T>() where T : ActiveSkill
        {
            foreach (var s in activeSkills.Values)
            {
                if (s is T)
                    return s as T;
            }

            return null;
        }

        // 获取默认主动技能
        public ActiveSkill GetDefaultActiveSkill()
        {
            return defaultSkillName == null ? null : GetActiveSkillByName(defaultSkillName);
        }

        // 移除主动技能
        public void RemoveActiveSkill(ActiveSkill skill)
        {
            Debug.Assert(skill.Owner == this, "skill named: " + skill.ID + " doest not exist.");
            RemoveActiveSkill(skill.ID);
        }

        // 移除主动技能
        public void RemoveActiveSkill(string name)
        {
            Debug.Assert(activeSkills.ContainsKey(name) && activeSkills[name].Owner == this, "skill named: " + name + " doest not exist.");
            activeSkills.Remove(name);
            if (defaultSkillName == name)
                defaultSkillName = null;
        }

        #endregion

        #region buff 相关

        Dictionary<string, PassiveSkill> pss = new Dictionary<string, PassiveSkill>();

        public PassiveSkill[] PassiveSkills { get => pss.Values.ToArray(); }
        public PassiveSkill GetPassiveSkillByID(string id) => pss.ContainsKey(id) ? pss[id] : null;

        public Buff[] Buffs
        {
            get
            {
                var buffs = new List<Buff>();
                foreach (var ps in pss.Values)
                    if (ps is Buff)
                        buffs.Add(ps as Buff);

                return buffs.ToArray();
            }
        }

        public void AddPassiveSkill(PassiveSkill ps)
        {
            Debug.Assert(GetPassiveSkillByID(ps.ID) == null, "passive skill duplicated: " + ps.ID);
            pss[ps.ID] = ps;
        }

        public void AddOrOverBuff(ref Buff buff)
        {
            if ((GetPassiveSkillByID(buff.ID) is Buff b))
            {
                if (b is CountDownBuff)
                    (b as CountDownBuff).Expand((buff as CountDownBuff).Num);

                if (b is ISkillWithOverlays)
                {
                    var ob = b as ISkillWithOverlays;
                    if (ob.Overlays < ob.MaxOverlays)
                    {
                        ob.Overlays++;
                        ob.UpdateOwnerAttrs();
                    }
                }

                buff = b;
            }
            else
                pss[buff.ID] = buff;
        }

        public void RemovePassiveSkill(PassiveSkill ps)
        {
            Debug.Assert(pss.ContainsKey(ps.ID) && pss[ps.ID] == ps, "buff " + ps.ID + " has not been attached to target " + ID);
            pss.Remove(ps.ID);
        }

        // 获取指定类型的 buff 或被动技能
        public T GetPassiveSkill<T>() where T : PassiveSkill
        {
            foreach (var ps in pss.Values)
                if (ps is T)
                    return ps as T;

            return null;
        }

        #endregion

        // 获取包括自己在内的同队队友
        public Warrior[] GetTeamMembers(bool includingSelf = false)
        {
            var members = new List<Warrior>();
            Battle.Map.ForeachObjs<Warrior>((x, y, w) =>
            {
                if (includingSelf || w != this)
                    members.Add(w);
            });

            return members.ToArray();
        }
    }
}