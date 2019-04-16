using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 所有 buff 效果基础结构
    /// </summary>
    public abstract class Buff
    {
        public virtual string Name { get; set; }
        public virtual Warrior Target { get; set; }
        public virtual Battle Battle { get { return Target?.Map?.Battle; } }
        public virtual void OnAttached() { }
        public virtual void OnDetached() { }
    }

    /// <summary>
    /// 回合计数的 buff
    /// </summary>
    public abstract class BuffCountDown : Buff
    {
        public int Num { get; set; } = 0;

        IEnumerator CountDown(int player)
        {
            if (player != Target.Owner)
                yield break;

            Num--;
            if (Num <= 0)
                yield return Battle.RemoveBuff(this);
        }

        public override void OnAttached()
        {
            Battle.BeforeActionDone.Add(CountDown);
        }

        public override void OnDetached()
        {
            Battle.BeforeActionDone.Del(CountDown);
        }
    }
}
