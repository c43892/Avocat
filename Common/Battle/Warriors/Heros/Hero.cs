using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 所有英雄角色
    /// </summary>
    public abstract class Hero : Warrior
    {
        public Hero(Battle bt)
            : base(bt.Map)
        {
            SetupBuffAndSkills();
        }

        protected abstract void SetupBuffAndSkills();
    }
}
