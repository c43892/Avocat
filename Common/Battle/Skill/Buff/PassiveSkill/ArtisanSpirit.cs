using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 洛里斯
    /// 匠心，行动阶段前，为团队提供能量
    /// </summary>
    public class ArtisanSpirit : PassiveSkill
    {
        IEnumerator AddEN(int player)
        {
            if (player != Owner.Owner)
                yield break;

            var bt = Battle as BattlePVE;
            yield return bt.AddEN(10);
        }

        public override IEnumerator OnAttached()
        {
            Battle.BeforeStartNextRound.Add(AddEN);
            yield return base.OnAttached();
        }

        public override IEnumerator OnDetached()
        {
            Battle.BeforeStartNextRound.Del(AddEN);
            yield return null;
        }
    }
}
