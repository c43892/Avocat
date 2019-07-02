using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 符文，影响英雄技能
    /// </summary>
    public abstract class Rune
    {
        public abstract string DisplayName { get; } // 显示名称

        // 战斗准备，符文可能会增加技能，替换技能或者修改技能效果
        public abstract void OnPreparingBattle();

        // 装备者
        public Hero Owner { get; set; }
    }
}
