using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 无视攻击距离进行反击
    /// </summary>
    public class BackNoRange : EquipAttr
    {
        public override void OnPreparingBattle(Battle bt)
        {
            bt.PrepareAttack += (attacker, target, addtionalTargets, skill, attackFlags) =>
            {
                if (attacker.Team != Team)
                    return;

                if (attackFlags.Contains("CounterAttack") && !attackFlags.Contains("IgnoreAttackRange"))
                    attackFlags.Add("IgnoreAttackRange");
            };
        }
    }
}
