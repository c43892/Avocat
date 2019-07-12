using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 装备对象属性，效果类似符文
    /// </summary>
    public abstract class EquipAttr
    {
        // 所属队伍
        public int Team { get; set; }

        // 战斗准备，添加各种 buff 之类的东西
        public abstract void OnPreparingBattle(Battle bt);
    }
}
