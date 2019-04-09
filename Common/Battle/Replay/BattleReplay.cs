using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 可存储传输的游戏战斗录像数据
    /// </summary>
    public class BattleReplay : SerializableData
    {
        public long Time;
        public List<byte[]> Messages = new List<byte[]>();

        protected override void Sync()
        {
            BeginSync();
            SyncLong(ref Time);

            if (IsWrite)
            {
                W.Write(Messages.Count);
                FC.ForEach(Messages, (i, msg) =>
                {
                    W.Write(msg.Length);
                    W.Write(msg);
                });
            }
            else
            {
                Messages.Clear();

                var count = R.ReadInt();
                FC.For(count, (i) =>
                {
                    var dataLen = R.ReadInt();
                    var data = R.ReadBytes(dataLen);
                    Messages.Add(data);
                });
            }

            EndSync();
        }
    }
}
