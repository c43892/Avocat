using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public interface ITacticalCommandImpl
    {
        void OnBeforeStartNextRound(int team);
        BaLuoKe Owner { set; }
    }

    /// <summary>
    /// 巴洛克，战术指挥
    /// </summary>
    public class TacticalCommand : BornBuff
    {
        public override string ID { get; } = "TacticalCommand";
        public TacticalCommand(Warrior owner) : base(owner) { }

        public ITacticalCommandImpl Impl
        {
            get
            {
                return impl;
            }
            set
            {
                impl = value;
                if (impl != null)
                    impl.Owner = Owner as BaLuoKe;
            }
        } ITacticalCommandImpl impl;

        void OnBeforeStartNextRound(int team)
        {
            Impl.OnBeforeStartNextRound(team);
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += OnBeforeStartNextRound;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeStartNextRound -= OnBeforeStartNextRound;
            base.OnDetached();
        }
    }

    /// <summary>
    /// 巴洛克
    /// 战术指挥，行动阶段前，生成一张指令卡
    /// </summary>
    public class TacticalCommandImpl1 : ITacticalCommandImpl
    {
        public BaLuoKe Owner { get; set; }

        public Func<string[]> GetCardTypes { get; set; }
        public TacticalCommandImpl1(Func<string[]> getCardTypes)
        {
            GetCardTypes = getCardTypes;
        }

        public void OnBeforeStartNextRound(int team)
        {
            if (team != Owner.Team)
                return;

            var bt = Owner.Battle as BattlePVE;
            foreach (var c in GetCardTypes())
                bt.AddBattleCard(BattleCard.Create(c));
        }
    }

    /// <summary>
    /// 巴洛克
    /// 战术指挥，行动阶段前，全体魔力和攻击力提升一层
    /// </summary>
    public class TacticalCommandImpl2 : ITacticalCommandImpl
    {
        public BaLuoKe Owner { get; set; }

        public void OnBeforeStartNextRound(int team)
        {
            if (team != Owner.Team)
                return;

            foreach (var m in Owner.GetTeamMembers())
            {
                Owner.Battle.AddBuff(new POWInc(Owner, 2, 1));
                Owner.Battle.AddBuff(new ATKInc(Owner, 2, 1));
            }
        }
    }
}
