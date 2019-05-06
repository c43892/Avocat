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

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override void Fire()
        {
            var owner = Warrior as BaLuoKe;
            Debug.Assert(owner != null, "only BaLuoKe can use this skill");
            Battle.Transform(Warrior, owner.State == "Lancer" ? "Archer" : "Lancer");
        }
    }
}
