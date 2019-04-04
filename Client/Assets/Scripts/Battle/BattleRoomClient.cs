using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;

/// <summary>
/// 客户端战斗对象
/// </summary>
public class BattleRoomClient : BattleRoom
{
    public int PlayerMe { get; private set; }

    public BattleRoomClient(Battle bt)
        :base(bt)
    {
    }

    // 交换英雄位置
    public void ExchangeWarroirsPosition(int fromX, int fromY, int toX, int toY)
    {
        ExchangeWarroirsPosition(PlayerMe, fromX, fromY, toX, toY);
    }

    // 战斗准备完毕
    public void Prepared()
    {
        PlayerPrepared(PlayerMe);
    }
}