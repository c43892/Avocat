using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public interface ISkillWithAXY // 一种伤害计算类型，具体详见数值文档
    {
        int A { get; }
        int X { get; }
        int Y { get; }
    }

    /// <summary>
    /// 所有主被动技能
    /// </summary>
    public abstract class Skill
    {
        public virtual string Name { get; } // 每组技能必须唯一，可能存在比如 蝶舞 和 蝶舞AOE 两个技能共享一个 Name
        public virtual string DisplayName { get;}
        public virtual Warrior Owner { get; set; } // 技能在哪个角色身上
        public virtual string SkillDescription { get; set; }
    }

    /// <summary>
    /// 所有 buff 效果基础结构
    /// </summary>
    public abstract class Buff : Skill
    {
        public virtual Battle Battle { get; set; } // 所属战斗对象
        public virtual BattleMap Map { get { return Battle?.Map; } }
        public virtual bool isBattleBUFF { get; set; }
        public virtual void OnAttached() { }
        public virtual void OnDetached() { }
    }

    /// <summary>
    /// 回合计数的 buff
    /// </summary>
    public abstract class BuffCountDown : Buff
    {
        public int MaxNum { get; set; } = 0;
        public int Num { get; set; } = 0;

        public BuffCountDown(int num)
        {
            Num = num;
        }

        // 叠加回合数
        public void Expand(int addtionalNum)
        {
            Num = (Num + addtionalNum).Clamp(0, MaxNum);
        }

        void CountDown(int player)
        {
            if (player != Owner.Team)
                return;

            Num--;
            if (Num <= 0)
                Battle.RemoveBuff(this);
        }

        public override void OnAttached()
        {
            Battle.BeforeActionDone += CountDown;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeActionDone -= CountDown;
            base.OnDetached();
        }
    }
}
