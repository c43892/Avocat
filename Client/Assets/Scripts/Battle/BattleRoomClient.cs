using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;
using System.Diagnostics;

public interface IBattleMessageSender
{
    void Send(string op, Action<IWriteableBuffer> data = null);
}

/// <summary>
/// 客户端战斗对象
/// </summary>
public class BattleRoomClient : BattleRoom
{
    // 自己在房间中的 player 编号
    public int PlayerMe { get; set; }

    // 负责消息发送
    public IBattleMessageSender BMS { get; set; }

    public BattleRoomClient(Battle bt)
        :base(bt)
    {
    }

    // 战斗准备完毕
    public void DoPrepared()
    {
        BMS.Send("PlayerPrepared");
    }

    // 交换角色位置
    public void DoExchangeWarroirsPosition(int fx, int fy, int tx, int ty)
    {
        BMS.Send("ExchangeWarroirsPosition", (data) =>
        {
            data.Write(fx);
            data.Write(fy);
            data.Write(tx);
            data.Write(ty);
        });
    }

    // 沿路径移动角色
    public void DoMoveOnPath(Warrior warrior)
    {
        BMS.Send("MoveOnPath", (data) =>
        {
            warrior.GetPosInMap(out int x, out int y);
            data.Write(x);
            data.Write(y);
            data.Write(warrior.MovingPath.ToArray());
        });
    }

    // 执行攻击操作
    public void DoAttack(Warrior attacker, Warrior target)
    {
        Debug.Assert(attacker.Owner == PlayerMe, "attacker should be in my team");
        BMS.Send("Attack", (data) =>
        {
            attacker.GetPosInMap(out int fx, out int fy);
            target.GetPosInMap(out int tx, out int ty);
            data.Write(fx);
            data.Write(fy);
            data.Write(tx);
            data.Write(ty);
        });
    }

    // 回合结束
    public void ActionDone()
    {
        BMS.Send("ActionDone");
    }
}
