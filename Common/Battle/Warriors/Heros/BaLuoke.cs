using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 巴洛克
    /// 战术指挥
    /// </summary>
    public class BaLuoKe : Hero, ITransformable
    {
        public BaLuoKe(Battle bt)
            : base(bt)
        {
            Name = "巴洛克";
        }

        public int ArcherAttackRange { get; set; }
        public int ArcherATK { get; set; }
        public int LancerAttackRange { get; set; }
        public int LancerATK { get; set; }

        public string State {
            get
            {
                return state;
            }
            set
            {
                state = value;
                switch (value)
                {
                    case "Archer": // 弓手
                        AttackRange = ArcherAttackRange;
                        ATK = ArcherATK;
                        break;
                    case "Lancer": // 枪兵
                        AttackRange = LancerAttackRange;
                        ATK = LancerATK;
                        break;
                }
            }
        } string state;

        protected override void SetupBuffAndSkills()
        {
            FC.Async2Sync(Battle.AddBuff(new TacticalCommand(() => State == "Archer" ? "EN" : "ATK"), this)); // 战术指挥
            AddActiveSkill(new FastAssistance()); // 快速援护
        }
    }
}
