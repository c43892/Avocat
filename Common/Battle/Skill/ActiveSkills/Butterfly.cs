using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗所有友方单位50%最大生命值
    /// </summary>
    public class Butterfly : ActiveSkill
    {
        public override string Name { get => "Butterfly"; }

        // 能量消耗
        public override int EnergyCost { get => 50; }

        // 主动释放
        public override IEnumerator Fire()
        {
            var map = Battle.Map;
            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var warrior = map.GetWarriorAt(x, y);
                    if (warrior == null || warrior.Owner != Owner.Owner)
                        continue;

                    yield return Battle.AddHP(warrior, warrior.MaxHP / 2);
                }
            }
        }
    }
}
