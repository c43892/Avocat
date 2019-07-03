using System.Collections;
using System.Collections.Generic;
using Swift;

namespace Avocat
{
    /// <summary>
    /// buff/debuff
    /// </summary>
    public abstract class Buff : Skill
    {
        public virtual void OnAttached() { }
        public virtual void OnDetached() { }
    }

    public abstract class BuffWithOwner : Buff, ISkillWithOwner
    {
        public override Battle Battle => Owner.Battle;
        public Warrior Owner { get; }
        public BuffWithOwner(Warrior owner)
        {
            Owner = owner;
        }
    }

    public abstract class BattleBuff : Buff
    {
        public override Battle Battle { get; }
        public BattleBuff(Battle bt)
        {
            Battle = bt;
        }
    }
}
