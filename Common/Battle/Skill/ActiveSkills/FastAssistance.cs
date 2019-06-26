using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Avocat
{
    /// <summary>
    /// 巴洛克
    /// 快速援护，切换巴洛克形态
    /// </summary>
    public class FastAssistance : ActiveSkill
    {
        public override string Name { get => "FastAssistance"; }
        public override string DisplayName { get => "快速援护"; }
        public override string SkillDescription { get; set; } = "切换巴洛克形态";

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override void Fire()
        {
            var owner = Owner as BaLuoKe;
            Debug.Assert(owner != null, "only BaLuoKe can use this skill");
            Battle.Transform(Owner, owner.State == "Lancer" ? "Archer" : "Lancer");
        }
    }
}
