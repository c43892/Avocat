using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    public interface IBattlemessageProvider
    {
        void HandleMsg(string msg, Action<IReadableBuffer> handler);
    }

    public static class BattleUtils
    {
        // 注册所有战斗消息
        public static void RegisterBattleMessageHandlers(this BattleRoom room, IBattlemessageProvider bmp)
        {
            bmp.HandleMsg("ExchangeWarroirsPosition", (data) =>
            {
                var fx = data.ReadInt();
                var fy = data.ReadInt();
                var tx = data.ReadInt();
                var ty = data.ReadInt();
                room.ExchangeWarroirsPosition(fx, fy, tx, ty);
            });

            bmp.HandleMsg("PlayerPrepared", (data) =>
            {
                var player = data.ReadInt();
                room.PlayerPrepared(player);
            });

            bmp.HandleMsg("MoveOnPath", (data) =>
            {
                var x = data.ReadInt();
                var y = data.ReadInt();
                var pathXYArr = data.ReadIntArr();

                var warrior = room.Battle.Map.Warriors[x, y];
                warrior.MovingPath.Clear();
                warrior.MovingPath.AddRange(pathXYArr);

                room.MoveOnPath(warrior);
            });

            bmp.HandleMsg("Attack", (data) =>
            {
                var fx = data.ReadInt();
                var fy = data.ReadInt();
                var tx = data.ReadInt();
                var ty = data.ReadInt();

                var attacker = room.Battle.Map.Warriors[fx, fy];
                var target = room.Battle.Map.Warriors[tx, ty];

                room.Attack(attacker, target);
            });
        }
    }
}
