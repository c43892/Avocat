using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using System.Diagnostics;

namespace Avocat
{
    public interface IBattlemessageProvider
    {
        void HandleMsg(string msg, Action<int, IReadableBuffer> handler);
    }

    /// <summary>
    /// 战斗房间，对接玩家操作，并处理除了战斗本身以外的逻辑，比如结算，观战，断线重连等
    /// </summary>
    public class BattleRoom
    {
        public Battle Battle { get; private set; }

        // 战斗准备情况
        public BattleRoom(Battle bt)
        {
            Battle = bt;
        }

        // 注册所有战斗消息
        public void RegisterBattleMessageHandlers(IBattlemessageProvider bmp)
        {
            bmp.HandleMsg("ExchangeWarroirsPosition", (player, data) =>
            {
                var fx = data.ReadInt();
                var fy = data.ReadInt();
                var tx = data.ReadInt();
                var ty = data.ReadInt();
                Battle.ExchangeWarroirsPosition(fx, fy, tx, ty);
            });

            bmp.HandleMsg("PlayerPrepared", (player, data) =>
            {
                Battle.PlayerPrepared(player);
            });

            bmp.HandleMsg("MoveOnPath", (player, data) =>
            {
                var id = data.ReadInt();
                var pathXYArr = data.ReadIntArr();

                var warrior = Battle.Map.GetWarriorsByID(id);
                warrior.MovingPath.Clear();
                warrior.MovingPath.AddRange(pathXYArr);

                Battle.MoveOnPath(warrior);
            });

            bmp.HandleMsg("Attack", (player, data) =>
            {
                var attackerID = data.ReadInt();
                var targetID = data.ReadInt();

                var attacker = Battle.Map.GetWarriorsByID(attackerID);
                var target = Battle.Map.GetWarriorsByID(targetID);

                attacker.GetPosInMap(out int fx, out int fy);
                target.GetPosInMap(out int tx, out int ty);
                if (MU.ManhattanDist(fx, fy, tx, ty) > attacker.AttackRange) // 超过攻击范围
                    return;

                Battle.Attack(attacker, target);
            });

            bmp.HandleMsg("ActionDone", (player, data) =>
            {
                Battle.ActionDone(player);
            });
        }
    }
}
