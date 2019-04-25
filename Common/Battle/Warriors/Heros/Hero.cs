using Swift;
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
        }

        protected void SetupBuffAndSkills(ActiveSkill a, params Buff[] bs)
        {
            if (a != null)
                AddActiveSkill(Configuration.Config(a));

            foreach (var b in bs)
                FC.Async2Sync(Battle.AddBuff(Configuration.Config(b), this));
        }
    }
}
