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
    /// 树干
    /// 使目标获得创伤效果
    /// </summary>
    public class Trunk : UsableItem
    {
        public Trunk(BattleMap map)
            : base(map)
        {
            Name = "树干";
        }

        // 对指定位置使用
        public override IEnumerator Use2(Warrior target)
        {
            if (target != null)
                yield return Battle.AddBuff(new Untreatable(2), target);
        }
    }
}
