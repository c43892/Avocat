using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 游川隐
    /// 剑道，攻击时施加创伤效果，使目标不可被治疗
    /// </summary>
    public class Kendo : PassiveSkill
    {
        // 效果持续几回合
        public int EffectRoundNum { get; set; }

        IEnumerator OnAfterAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (attacker != Warrior)
                yield break;

            yield return Battle.AddBuff(new Untreatable(EffectRoundNum), target);
        }

        public override IEnumerator OnAttached()
        {
            Battle.AfterAttack.Add(OnAfterAttack);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.AfterAttack.Del(OnAfterAttack);
            yield return base.OnDetached();
        }
    }
}
