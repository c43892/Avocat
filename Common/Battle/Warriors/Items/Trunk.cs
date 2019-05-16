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
            EnglishName = "Trunk";
        }
        
        public int EffectRoundNum { get; set; }

        // 对指定位置使用
        public override void Use2(Warrior target)
        {
            if (target != null)
                Battle.AddBuff(Configuration.Config(new Untreatable(EffectRoundNum)), target);
        }
    }
}
