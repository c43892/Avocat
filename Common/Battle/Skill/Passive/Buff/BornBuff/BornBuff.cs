namespace Avocat
{
    /// <summary>
    /// 天赋技能
    /// </summary>
    public abstract class BornBuff : BuffWithOwner
    {
        public BornBuff(Warrior owner) : base(owner) { }
    }
}
