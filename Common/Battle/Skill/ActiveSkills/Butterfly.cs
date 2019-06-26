using Swift.Math;
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
    /// 蝶舞，治疗单个友方单位 50% 最大生命值
    /// </summary>
    public class Butterfly : ActiveSkill
    {
        public override string Name { get => "Butterfly"; }
        public override string DisplayName { get => "蝶舞"; }
        public override string SkillDescription { get; set; } = "治疗单个友方单位 50% 最大生命值";

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override void Fire()
        {
            Warrior target = null;
            var map = Battle.Map;
            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var warrior = map.GetAt<Warrior>(x, y);
                    if (warrior == null || warrior.Team != Warrior.Team)
                        continue;

                    if (target != null)
                    {
                        // 选择血量百分比最小的目标
                        var hpPercentage1 = (Fix64)target.HP / target.MaxHP;
                        var hpPercentage2 = (Fix64)warrior.HP / warrior.MaxHP;
                        if (hpPercentage2 < hpPercentage1)
                            target = warrior;
                    }
                }
            }

            if (target != null)
                Battle.AddHP(target, target.MaxHP / 2);
        }
    }

    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗所有友方单位 50% 最大生命值
    /// </summary>
    public class ButterflyAOE : ActiveSkill
    {
        public override string Name { get => "Butterfly"; }
        public override string DisplayName { get => "蝶舞"; }
        public override string SkillDescription { get; set; } = "治疗所有友方单位 50% 最大生命值";

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override void Fire()
        {
            var map = Battle.Map;
            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var warrior = map.GetAt<Warrior>(x, y);
                    if (warrior == null || warrior.Team != Warrior.Team)
                        continue;

                    Battle.AddHP(warrior, warrior.MaxHP / 2);
                }
            }
        }
    }
}
