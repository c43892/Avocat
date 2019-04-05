using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avocat;
using Swift;
using System.Diagnostics;

/// <summary>
/// 战斗消息回环，用于模拟网络消息从服务器到客户端的往返
/// </summary>
public class BattleMessageLooper : IBattlemessageProvider, IBattleMessageSender
{
    RingBuffer msgBuff = new RingBuffer();
    public void Send(string op, Action<IWriteableBuffer> w)
    {
        msgBuff.Write(op);
        w(msgBuff);
    }

    Dictionary<string, Action<IReadableBuffer>> hs = new Dictionary<string, Action<IReadableBuffer>>();
    public void OnMsg(string msg, Action<IReadableBuffer> r)
    {
        hs[msg] = r;
    }

    public void MoveNext()
    {
        if (msgBuff.Available <= 0)
            return;

        var op = msgBuff.ReadString();
        Debug.Assert(hs.ContainsKey(op), "no handler for message: " + op);
        hs[op](msgBuff);
    }
}
