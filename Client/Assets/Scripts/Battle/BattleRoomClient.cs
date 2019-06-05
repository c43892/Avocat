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
/// 客户端战斗房间
/// </summary>
public class BattleRoomClient
{
    // 自己在房间中的 player 编号
    public int PlayerMe { get; set; }

    public BattleRoom BattleRoom { get; private set; }

    // 战斗对象
    public Battle Battle { get { return BattleRoom.Battle; } }

    // 负责消息发送
    public IBattleMessageSender BMS { get; set; }

    public BattleRoomClient(BattleRoom room)
    {
        BattleRoom = room;
    }

    public void RegisterBattleMessageHandlers(IBattlemessageProvider bmp)
    {
        BattleRoom.RegisterBattleMessageHandlers(bmp);
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
            data.Write(warrior.IDInMap);
            data.Write(warrior.MovingPath.ToArray());
        });
    }

    // 执行攻击操作
    public void DoAttack(Warrior attacker, Warrior target)
    {
        Debug.Assert(attacker.Team == PlayerMe, "attacker should be in my team");
        BMS.Send("Attack", (data) =>
        {
            data.Write(attacker.IDInMap);
            data.Write(target.IDInMap);
        });
    }

    // 回合结束
    public void ActionDone()
    {
        BMS.Send("ActionDone");
    }

    // 交换战斗卡牌位置
    public void DoExchangeBattleCards(int g1, int n1, int g2, int n2)
    {
        BMS.Send("ExchangeBattleCards", (data) =>
        {
            data.Write(g1);
            data.Write(n1);
            data.Write(g2);
            data.Write(n2);
        });
    }

    // 自动排序卡牌
    public void SortingBattleCards(Warrior warrior)
    {
        BMS.Send("SortingBattleCards", (data) =>
        {
            data.Write(warrior.IDInMap);
        });
    }

    // 释放主动技能
    public void FireActiveSkill(ActiveSkill skill)
    {
        var warrior = skill.Warrior;
        BMS.Send("FireActiveSkill", (data) =>
        {
            data.Write(warrior.IDInMap);
            data.Write(skill.Name);
        });
    }

    // 释放主动技能，带目标
    public void FireActiveSkillAt(ActiveSkill skill, int x, int y)
    {
        var warrior = skill.Warrior;
        BMS.Send("FireActiveSkillAt", (data) =>
        {
            data.Write(warrior.IDInMap);
            data.Write(skill.Name);
            data.Write(x);
            data.Write(y);
        });
    }

    // 对目标使用道具
    public void UseItem2(Avocat.ItemOnMap item, Warrior target)
    {
        BMS.Send("UseItem2", (data) =>
        {
            data.Write(item.IDInMap);
            data.Write(target.IDInMap);
        });
    }
}
