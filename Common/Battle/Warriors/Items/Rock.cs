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
    public class Rock : ItemOnMap
    {
        public Rock(BattleMap map)
            : base(map)
        {
            ID = "Rock";
        }

        public int EffectRoundNum { get; set; }

        // 对指定位置使用
        public override void Use2(Warrior target)
        {
            if (target != null)
                Battle.AddBuff(new Faint(target, EffectRoundNum));
        }
    }
}
