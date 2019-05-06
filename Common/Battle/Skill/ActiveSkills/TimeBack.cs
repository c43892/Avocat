using Swift;
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
        public override void Fire()
        {
            var msgs = Battle.Replay.Messages;
            if (msgs.Count == 0)
                return;

            var i = 0;
            var actionDownCount = 0;
            while (true)
            {
                if (i >= msgs.Count)
                    break;

                var msg = msgs[msgs.Count - i - 1];
                var op = (new RingBuffer(msg)).ReadString();

                // 消息往回退到 PlayerPrepared 或者上上个 ActionDone 为止
                if (op == "PlayerPrepared")
                    break;
                else if (op == "ActionDone")
                {
                    actionDownCount++;
                    if (actionDownCount == 2)
                        break;
                }

                i++;
            }

            if (i > 1)
            {
                Battle.Replay.Messages.RemoveRange(Battle.Replay.Messages.Count - i, i);
                Battle.TriggerTimeBack();
            }
        }
    }
}
