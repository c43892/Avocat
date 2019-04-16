using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 回合开始重置护盾
    /// </summary>
    public class ResetES : Buff
    {
        public override void OnAttached()
        {
            Battle.BeforeStartNextRound.Add((int player) =>
            {
                Map.ForeachWarriors((i, j, warrior) =>
                {
                    if (warrior.Owner != player)
                        return;

                    warrior.ES = 0; // 重置所有护甲
                });
            });
        }
    }
}
