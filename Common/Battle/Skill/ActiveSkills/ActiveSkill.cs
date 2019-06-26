using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 主动释放技能
    /// </summary>
    public abstract class ActiveSkill : Skill
    {
        public virtual Battle Battle { get => Owner.Battle; } // 所属战斗对象
        public virtual BattleMap Map { get { return Battle?.Map; } }
        // 能量消耗
        public abstract int EnergyCost { get; set; }

        // 主动释放
        public virtual void Fire() { throw new Exception("not implemented yet"); }

        // 主动释放
        public virtual void FireAt(int x, int y) { throw new Exception("not implemented yet"); }

        public virtual string ActiveSkillType { get; } = "fire"; // or fireAt
    }
}
