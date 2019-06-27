using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public abstract class Butterfly : ActiveSkill
    {
        public override string Name { get => "Butterfly"; }
        public override string DisplayName { get => "蝶舞"; }

        // 是否在释放蝶舞之后，添加一张药水卡，这个开关会被符文打开
        public bool AddOnePTCard { get; set; } = false;
        public void Try2AddOnePTCard()
        {
            if (!AddOnePTCard)
                return;

            var bt = Battle as BattlePVE;
            bt.AddBattleCard(new BattleCardPotion());
        }
    }

    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗单个友方单位 50% 最大生命值
    /// </summary>
    public class ButterflySingle : Butterfly
    {
        public override string ActiveSkillType { get; } = "fireAt";
        public override string SkillDescription { get; set; } = "治疗单个友方单位 50% 最大生命值";

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override void FireAt(int x, int y)
        {
            Warrior target = Owner.Battle.Map.GetAt<Warrior>(x, y);
            if (target == null || target.Team != Owner.Team)
                return;

            Battle.AddHP(target, target.MaxHP / 2);
            Try2AddOnePTCard();
        }
    }

    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗所有友方单位 50% 最大生命值
    /// </summary>
    public class ButterflyAOE : Butterfly
    {
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
                    if (warrior == null || warrior.Team != Owner.Team)
                        continue;

                    Battle.AddHP(warrior, warrior.MaxHP / 2);
                }
            }

            Try2AddOnePTCard();
        }
    }
}
