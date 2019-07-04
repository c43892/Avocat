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
    public abstract class ActiveSkill : Skill, ISkillWithOwner
    {
        public ActiveSkill(Warrior owner) { Owner = owner; }

        public Warrior Owner { get; private set; } // 技能所有者
        public override Battle Battle { get => Owner.Battle; } // 所属战斗对象
        public virtual int EnergyCost { get; set; } // 能量消耗
        public abstract void Fire(); // 主动释放
    }
}
