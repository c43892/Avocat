using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 回合开始重置行动标记
    /// </summary>
    public class ResetActionFlag : Buff
    {
        public override void OnAttached()
        {
            Battle.BeforeStartNextRound.Add((int player) =>
            {
                Map.ForeachWarriors((i, j, warrior) =>
                {
                    if (warrior.Owner != player)
                        return;

                    warrior.Moved = false; // 重置行动标记
                    warrior.ActionDone = false; // 重置行动标记
                });
            });
        }
    }
}
