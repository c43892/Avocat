using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public abstract class Butterfly : ActiveSkill, ISkillWithTargetFilter
    {
        public override string ID { get => "Butterfly"; }
        public Butterfly(Warrior owner) : base(owner) { }

        // 是否在释放蝶舞之后，添加一张药水卡，这个开关会被符文打开
        public bool AddOnePTCard { get; set; } = false;
        public void Try2AddOnePTCard()
        {
            if (!AddOnePTCard)
                return;

            var bt = Battle as BattlePVE;
            bt.AddBattleCard(new BattleCardPotion());
        }

        // 只能治疗队友和自己
        public bool TargetFilter(BattleMapObj target) => this.IsTeammate(target);
    }

    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗单个友方单位 50% 最大生命值
    /// </summary>
    public class ButterflySingle : Butterfly, ISkillWithPosSel
    {
        public ButterflySingle(Warrior owner) : base(owner) { }
        public int TX { get; set; }
        public int TY { get; set; }

        // 主动释放
        public override void Fire()
        {
            var tar = this.Target();
            if (!TargetFilter(tar))
                return;

            Battle.AddHP(tar, tar.MaxHP / 2);
            Try2AddOnePTCard();
        }
    }

    /// <summary>
    /// 黛丽万
    /// 蝶舞，治疗所有友方单位 50% 最大生命值
    /// </summary>
    public class ButterflyAOE : Butterfly, ISkillWithTargetFilter
    {
        public ButterflyAOE(Warrior owner) : base(owner) { }

        // 主动释放
        public override void Fire()
        {
            foreach(var tar in this.AllAvaliableTargets())
                Battle.AddHP(tar, tar.MaxHP / 2);

            Try2AddOnePTCard();
        }
    }
}
