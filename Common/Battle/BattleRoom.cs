using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.AStar;
using Swift.Math;
using System.Diagnostics;
using System.Collections;

namespace Avocat
{
    public interface IBattlemessageProvider
    {
        void HandleMsg(string msg, Action<int, IReadableBuffer> handler);
        Action<int, byte[]> OnMessageIn { get; set; }
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
        public event Action ReplayChanged = null;
        public virtual void RegisterBattleMessageHandlers(IBattlemessageProvider bmp)
        {
            bmp.OnMessageIn = (int seqNo, byte[] data) =>
            {
                if (seqNo > Battle.Replay.Messages.Count)
                {
                    Battle.Replay.Messages.Add(data);
                    ReplayChanged?.Invoke();
                }
            };

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

                var warrior = Battle.Map.GetByID<Warrior>(id);
                warrior.MovingPath.Clear();
                warrior.MovingPath.AddRange(pathXYArr);

                Battle.MoveOnPath(warrior);
            });

            bmp.HandleMsg("Attack", (player, data) =>
            {
                var attackerID = data.ReadInt();
                var targetID = data.ReadInt();

                var attacker = Battle.Map.GetByID<Warrior>(attackerID);
                var target = Battle.Map.GetByID<Warrior>(targetID);

                if(attacker is Hero)
                    Battle.Attack(attacker, target);
            });

            bmp.HandleMsg("MoveOnPathAndAttack", (player, data) =>
            {
                var attackerID = data.ReadInt();
                var pathXYArr = data.ReadIntArr();
                var targetID = data.ReadInt();

                var attacker = Battle.Map.GetByID<Warrior>(attackerID);
                attacker.MovingPath.Clear();
                attacker.MovingPath.AddRange(pathXYArr);
                var target = Battle.Map.GetByID<Warrior>(targetID);

                if (attacker is Hero)
                    Battle.MoveOnPathAndAttack(attacker, target);
            }); 

            bmp.HandleMsg("ActionDone", (player, data) =>
            {
                Battle.ActionDone(player);
            });
        }

        // 搜索可达路径
        public List<int> FindPath(int fx, int fy, int tx, int ty, int radius, TileType tileMask)
        {
            return Battle.Map.FindPath(fx, fy, tx, ty, radius, tileMask);
        }
    }
}
