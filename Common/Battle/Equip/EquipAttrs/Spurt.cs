using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 溅射伤害
    /// </summary>
    public class Spurt : EquipAttr
    {
        public int P0 { get; set; } // 溅射伤害系数

        public override void OnPreparingBattle(Battle bt)
        {
            bt.BeforeAttack+= (attacker, target, addTars, skill, attackFlags) =>
            {
                if (attacker.Team != Team)
                    return;

                target.GetPosInMap(out int cx, out int cy);
                FC.ObliqueSquareFor(cx, cy, 1, (x, y) =>
                {
                    if (x >= 0 && y >= 0 && x < bt.Map.Width && y < bt.Map.Height)
                    {
                        var t = bt.Map.GetAt<Warrior>(x, y);
                        if (t != null && t.Team != Team && !t.IsDead)
                            addTars.Add(t);
                    }
                });
            };
        }
    }
}
