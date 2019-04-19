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
        public virtual Warrior Owner { get; set; }
        public virtual Battle Battle { get { return Owner?.Battle; } }
        public virtual BattleMap Map { get { return Battle?.Map; } }

        // 能量消耗
        public abstract int EnergyCost { get; }

        // 主动释放
        public abstract IEnumerator Fire();
    }
}
