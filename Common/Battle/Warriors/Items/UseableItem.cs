using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 地表物件，可以被英雄使用，有一定类似技能的效果
    /// </summary>
    public abstract class UsableItem : BattleMapItem
    {
        public UsableItem(BattleMap map)
            : base(map)
        {
        }

        // 对指定位置使用
        public virtual IEnumerator Use2(Warrior target) { throw new Exception("not implemented yet"); }
    }
}
