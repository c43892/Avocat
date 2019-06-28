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
        string SkillDescription { get; }
        Func<BaLuoKe> GetOwner { set; }
    }

    /// <summary>
    /// 巴洛克，战术指挥
    /// </summary>
    public class TacticalCommand : PassiveSkill
    {
        public override string Name { get; } = "TacticalCommand";
        public override string DisplayName { get; } = "战术指挥";
        public override string SkillDescription { get => Impl.SkillDescription; }

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
                    impl.GetOwner = () => Owner as BaLuoKe;
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
        public Func<BaLuoKe> GetOwner { get; set; }
        public string SkillDescription { get; set; } = "行动阶段前，生成一张指令卡";

        public Func<string[]> GetCardTypes { get; set; }
        public TacticalCommandImpl1(Func<string[]> getCardTypes)
        {
            GetCardTypes = getCardTypes;
        }

        public void OnBeforeStartNextRound(int team)
        {
            var owner = GetOwner();
            if (team != owner.Team)
                return;

            var bt = owner.Battle as BattlePVE;
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
        public Func<BaLuoKe> GetOwner { get; set; }
        public string SkillDescription { get; set; } = "行动阶段前，不再提供指令卡，赋予全体友方单位攻击提升与魔力提升各一层";

        public void OnBeforeStartNextRound(int team)
        {
            var owner = GetOwner();
            if (team != owner.Team)
                return;

            var bt = owner.Battle as BattlePVE;
            var teammates = owner.GetTeamMembers();
            foreach (var m in teammates)
            {
                bt.AddBuff(new POWInc(1), owner);
                bt.AddBuff(new ATKInc(1), owner);
            }
        }
    }
}
