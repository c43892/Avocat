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
        public virtual string Name { get; }
        public virtual Warrior Target { get; set; }
        public virtual Battle Battle { get; set; }
        public virtual BattleMap Map { get { return Battle?.Map; } }
        public virtual IEnumerator OnAttached() { yield return null; }
        public virtual IEnumerator OnDetached() { yield return null; }
    }

    /// <summary>
    /// 回合计数的 buff
    /// </summary>
    public abstract class BuffCountDown : Buff
    {
        public int Num { get; set; } = 0;

        public BuffCountDown(int num)
        {
            Num = num;
        }

        IEnumerator CountDown(int player)
        {
            if (player != Target.Owner)
                yield break;

            Num--;
            if (Num <= 0)
                yield return Battle.RemoveBuff(this);
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeActionDone.Add(CountDown);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeActionDone.Del(CountDown);
            yield return base.OnDetached();
        }
    }
}
