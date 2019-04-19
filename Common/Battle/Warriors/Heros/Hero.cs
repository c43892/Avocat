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
        public Hero(Battle bt, int maxHP, int maxES)
            : base(bt.Map, maxHP, maxES)
        {
            SetupBuffAndSkills();
        }

        protected abstract void SetupBuffAndSkills();
    }
}
