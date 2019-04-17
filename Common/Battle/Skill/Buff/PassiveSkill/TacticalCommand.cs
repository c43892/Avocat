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

        IEnumerator AddBattleCard(int player)
        {
            if (player != Target.Owner)
                yield break;

            var bt = Battle as BattlePVE;
            var card = BattleCard.Create(GetCardType());
            yield return bt.AddBattleCard(card);
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeStartNextRound.Add(AddBattleCard);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeStartNextRound.Del(AddBattleCard);
            yield return null;
        }
    }
}
