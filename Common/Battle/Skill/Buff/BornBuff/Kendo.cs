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
    public class Kendo : BuffWithOwner
    {
        // 效果持续几回合
        public int EffectRoundNum { get; set; }
        public override string ID { get; } = "Kendo";
        public Kendo(Warrior owner) : base(owner) { }

        void OnAfterAttack(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            if (attacker != Owner)
                return;

            Battle.AddBuff(new Untreatable(Owner, EffectRoundNum));
        }

        public override void OnAttached()
        {
            Battle.AfterAttack += OnAfterAttack;
           base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterAttack -= OnAfterAttack;
            base.OnDetached();
        }
    }
}
