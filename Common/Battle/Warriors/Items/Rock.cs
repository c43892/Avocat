using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 岩石
    /// 使目标眩晕
    /// </summary>
    public class Rock : UsableItem
    {
        public Rock(BattleMap map)
            : base(map)
        {
            Name = "岩石";
        }

        // 对指定位置使用
        public override IEnumerator Use2(Warrior target)
        {
            if (target != null)
                yield return Battle.AddBuff(new Faint(2), target);
        }
    }
}
