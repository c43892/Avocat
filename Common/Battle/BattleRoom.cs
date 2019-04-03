using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace Battle
{
    /// <summary>
    /// 战斗房间，处理除了战斗本身以外的逻辑，比如结算，观战，断线重连等
    /// </summary>
    public class BattleRoom
    {
        public Battle Battle { get; private set; }

        protected StateMachineManager smm = new StateMachineManager();

        public BattleRoom(Battle bt)
        {
            Battle = bt;
        }

        // 交换英雄位置
        public virtual void ExchangeWarroirsPosition(int player, int fromX, int fromY, int toX, int toY)
        {
            Battle.ExchangeWarroirsPosition(player, fromX, fromY, toX, toY);
        }

        // 战斗准备结束
        public virtual void Prepared(int player)
        {
        }
    }
}
