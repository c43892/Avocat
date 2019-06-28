using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 洛里斯
    /// 匠心，行动阶段前，为团队提供能量
    /// </summary>
    public class ArtisanSpirit : PassiveSkill
    {
        public int ES2Add { get; set; }
        public override string Name { get; } = "ArtisanSpirit";
        public override string DisplayName { get; } = "匠心";
        public override string SkillDescription { get; set; } = "行动阶段前，为团队提供能量";

        // 效果翻倍系数
        public int EffectFactor { get; set; } = 1;

        void AddEN(int team)
        {
            if (team != Owner.Team)
                return;

            var bt = Battle as BattlePVE;
            bt.AddEN(ES2Add * EffectFactor);
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += AddEN;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeStartNextRound -= AddEN;
            base.OnDetached();
        }
    }
}
