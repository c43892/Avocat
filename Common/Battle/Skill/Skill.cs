using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public interface ISkillWithAXY // 一种伤害计算类型，具体详见数值文档
    {
        int A { get; }
        int X { get; }
        int Y { get; }
    }

    public interface ISkillWithOwner // 附着在某个地图对象身上的技能
    {
        Warrior Owner { get; }
    }

    public interface ISkillWithRange // 技能释放范围
    {
        int Range { get; }
    }

    public interface ISkillWithTargetFilter
    {
        bool TargetFilter(BattleMapObj target); // 目标过滤条件
    }

    public interface ISkillWithPosSel // 选择技能释放位置
    {
        int TX { get; set; } // 目标坐标 x
        int TY { get; set; } // 目标坐标 y
    }

    public interface ISkillWithOverlays // 有叠加层数逻辑
    {
        int Overlays { get; set; } // 叠加层数
        int MaxOverlays { get; } // 最大叠加层数
        void UpdateOwnerAttrs(); // 更新所有者属性
    }

    /// <summary>
    /// 主被动技能
    /// </summary>
    public abstract class Skill
    {
        public abstract string ID { get; } // 每组技能必须唯一，可能存在比如 蝶舞 和 蝶舞AOE 两个技能共享一个 Name
        public abstract Battle Battle { get; } // 所属战斗对象
        public virtual BattleMap Map { get { return Battle?.Map; } }
    }

    public static class SkillEx
    {
        // 技能目标位置上的对象
        public static Warrior Target<T>(this T skill) where T : Skill, ISkillWithPosSel => skill.Map.GetAt<Warrior>(skill.TX, skill.TY);

        // 地图上所有满足条件的攻击目标
        public static List<Warrior> AllAvaliableTargets<T>(this T skill) where T : ISkillWithTargetFilter
        {
            var targets = new List<Warrior>();
            (skill as Skill).Map.ForeachObjs((x, y, obj) =>
            {
                if (obj is Warrior && skill.TargetFilter(obj))
                    targets.Add(obj as Warrior);
            });

            return targets;
        }

        public static void FireAt<T>(this T skill, int x, int y) where T : ISkillWithPosSel
        {
            skill.TX = x;
            skill.TY = y;
            (skill as ActiveSkill).Fire();
        }

        // 判断自定对象是否是队友的函数
        public static bool IsTeammate<T>(this T skill, BattleMapObj target) where T : ISkillWithOwner => target is Warrior && (target as Warrior).Team == skill.Owner.Team;

        // 判断自定对象是否是敌人的函数
        public static bool EnemyChecker<T>(this T skill, BattleMapObj target) where T : ISkillWithOwner => target is Warrior && (target as Warrior).Team != skill.Owner.Team;
    }
}
