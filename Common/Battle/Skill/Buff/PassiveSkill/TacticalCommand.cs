using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 巴洛克
    /// 战术指挥，行动阶段前，生成一张指令卡
    /// </summary>
    public class TacticalCommand : PassiveSkill
    {
        Func<string> GetCardType { get; set; }
        public TacticalCommand(Func<string> getCardType)
        {
            GetCardType = getCardType;
        }

        void AddBattleCard(int player)
        {
            if (player != Warrior.Team)
                return;

            var bt = Battle as BattlePVE;
            var card = BattleCard.Create(GetCardType());
            bt.AddBattleCard(card);
        }

        public override void OnAttached()
        {
            Battle.BeforeStartNextRound += AddBattleCard;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeStartNextRound -= AddBattleCard;
            base.OnDetached();
        }
    }
}
