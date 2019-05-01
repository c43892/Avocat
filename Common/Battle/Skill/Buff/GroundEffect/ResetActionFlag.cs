using System;
using System.Collections;
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
        IEnumerator ImplResetActionFlags(int player)
        {
            for (var i = 0; i < Map.Width; i++)
            {
                for (var j = 0; j < Map.Height; j++)
                {
                    var warrior = Map.GetWarriorAt(i, j);
                    if (warrior?.Team != player)
                        continue;

                    // 重置行动标记
                    yield return Battle.ResetActionFlag(warrior);
                }
            }
        }

        public override IEnumerator OnAttached()
        {
            Battle.AfterActionDone.Add(ImplResetActionFlags);
            yield return base.OnAttached();
        }
    }
}
