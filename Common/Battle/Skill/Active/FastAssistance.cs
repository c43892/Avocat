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
    /// 快速援护
    /// </summary>
    public abstract class FastAssistance : ActiveSkill
    {
        public override string ID { get => "FastAssistance"; }
        public FastAssistance(Warrior owner) : base(owner) { }

        // 基础效果：增加一层魔力，额外行动一次
        public void BaseEffect()
        {
            Owner.Battle.AddBuff(new POWInc(Owner, 2, 1));
            Owner.ActionDone = false;
        }
    }

    /// <summary>
    /// 巴洛克
    /// 快速援护，切换巴洛克形态
    /// </summary>
    public class FastAssistance1 : FastAssistance
    {
        public FastAssistance1(Warrior owner) : base(owner) { }

        // 主动释放
        public override void Fire()
        {
            var owner = Owner as BaLuoKe;
            Debug.Assert(owner != null, "only BaLuoKe can use this skill");
            Battle.Transform(Owner, owner.State == "Lancer" ? "Archer" : "Lancer");
            BaseEffect();
        }
    }

    /// <summary>
    /// 巴洛克
    /// 巴洛克始终处于近战状态，技能效果变为加满护盾
    /// </summary>
    public class FastAssistance2 : FastAssistance
    {
        public FastAssistance2(Warrior owner) : base(owner) { }

        // 主动释放
        public override void Fire()
        {
            var owner = Owner as BaLuoKe;
            Debug.Assert(owner != null, "only BaLuoKe can use this skill");
            Debug.Assert(owner.State == "Archer", "BaLuoKe should in Archer state");
            Battle.AddES(owner, owner.MaxES); // 加满护盾
            BaseEffect();
        }
    }
}
