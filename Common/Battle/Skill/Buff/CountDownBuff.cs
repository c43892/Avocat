using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 回合计数的 buff
    /// </summary>
    public abstract class CountDownBuff : Buff
    {
        public int Team { get; }
        public int MaxNum { get; set; } = 0;
        public int Num { get; set; } = 0;

        public CountDownBuff(int team, int num)
        {
            Num = num;
            Team = team;
        }

        void CountDown(int team)
        {
            if (team != Team)
                return;

            Num--;
            if (Num <= 0)
                Battle.RemoveBuff(this);
        }

        bool attached = false; // 因为此类 buff 的 attach 效果可能只是叠加现有层数，需要特别处理
        public override void OnAttached()
        {
            if (attached)
                return;

            attached = true;
            Battle.BeforeActionDone += CountDown;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            attached = false;
            Battle.BeforeActionDone -= CountDown;
            base.OnDetached();
        }
    }

    public abstract class CountDownBuffWithOwner : CountDownBuff, ISkillWithOwner
    {
        public override Battle Battle => Owner.Battle;
        public Warrior Owner { get; }
        public CountDownBuffWithOwner(Warrior owner, int num)
            : base(owner.Team, num)
        {
            Owner = owner;
        }
    }
}
