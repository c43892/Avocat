using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// XXX
    /// 时光倒流，整个战斗退回到上一步状态
    /// </summary>
    public class TimeBack : ActiveSkill
    {
        public override string Name { get => "TimeBack"; }

        // 能量消耗
        public override int EnergyCost { get; set; }

        // 主动释放
        public override IEnumerator Fire()
        {
            Battle.Replay.Messages.RemoveAt(Battle.Replay.Messages.Count - 1);
            yield return Battle.TriggerTimeBack();
        }
    }
}
