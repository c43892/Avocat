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
    public class ResetActionFlag : BattleBuff
    {
        public override string ID { get => "ResetActionFlag"; }
        public ResetActionFlag(Battle bt) : base(bt) { }
            
        void ImplResetActionFlags(int team)
        {
            for (var i = 0; i < Map.Width; i++)
            {
                for (var j = 0; j < Map.Height; j++)
                {
                    var warrior = Map.GetAt<Warrior>(i, j);
                    if (warrior?.Team != team)
                        continue;

                    // 重置行动标记和英雄释放技能的标记
                    Battle.ResetActionFlag(warrior);
                }
            }
        }

        public override void OnAttached()
        {
            Battle.AfterActionDone += ImplResetActionFlags;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            throw new Exception("not implemented yet");
        }
    }
}
