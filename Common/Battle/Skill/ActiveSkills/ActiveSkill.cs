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
    public abstract class ActiveSkill
    {
        public abstract string Name { get; }
        public virtual Warrior Warrior { get; set; }
        public virtual Battle Battle { get { return Warrior?.Battle; } }
        public virtual BattleMap Map { get { return Battle?.Map; } }

        // 能量消耗
        public abstract int EnergyCost { get; set; }

        // 主动释放
        public virtual IEnumerator Fire() { throw new Exception("not implemented yet"); }

        // 主动释放
        public virtual IEnumerator FireAt(int x, int y) { throw new Exception("not implemented yet"); }

        public virtual string ActiveSkillType { get; } = "fire"; // or fireAt
    }
}
