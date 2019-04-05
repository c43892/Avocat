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
    /// <summary>
    /// 战斗房间，处理除了战斗本身以外的逻辑，比如结算，观战，断线重连等
    /// </summary>
    public class BattleRoom
    {
        public Battle Battle { get; private set; }

        // 尚未准备完成的玩家列表
        List<string> playerPreparedDown = new List<string>();

        public BattleRoom(Battle bt)
        {
            Battle = bt;
            FC.Travel(bt.Players, (PlayerInfo p) => playerPreparedDown.Add(p.ID));
        }

        // 交换英雄位置
        public event Action<int, int, int, int> OnWarriorPositionExchanged = null;
        public virtual void ExchangeWarroirsPosition(int fx, int fy, int toX, int toY)
        {
            Battle.ExchangeWarroirsPosition(fx, fy, toX, toY);
            OnWarriorPositionExchanged.SC(fx, fy, toX, toY);
        }

        // 完成战斗准备
        public event Action<int> OnPlayerPrepared = null; // 有玩家完成战斗准备
        public event Action OnAllPrepared = null; // 所有玩家完成战斗准备
        public virtual void PlayerPrepared(int player)
        {
            OnPlayerPrepared.SC(player);
            playerPreparedDown.Remove(Battle.Players[player].ID);
            if (playerPreparedDown.Count == 0)
                OnAllPrepared.SC();
        }

        // 角色沿路径移动
        public event Action<int, int, int, int> OnWarriorPositionChanged = null;
        public event Action<Warrior, List<int>> OnWarriorMovingOnPath = null; // 角色沿路径移动
        public virtual void DoMoveOnPath(Warrior warrior)
        {
            var lstPathXY = warrior.MovingPath;
            var fx = lstPathXY[0];
            var fy = lstPathXY[1];
            lstPathXY.RemoveRange(0, 2);
            Debug.Assert(warrior == Battle.Map.Warriors[fx, fy], "the warrior has not been right on the start position: " + fx + ", " + fy);

            List<int> movedPath = new List<int>(); // 实际落实了的移动路径
            movedPath.Add(fx);
            movedPath.Add(fy);

            while (lstPathXY.Count >= 2)
            {
                var tx = lstPathXY[0];
                var ty = lstPathXY[1];
                
                fx = tx;
                fy = ty;
                lstPathXY.RemoveRange(0, 2);
                movedPath.Add(fx);
                movedPath.Add(fy);

                Battle.ExchangeWarroirsPosition(fx, fy, tx, ty);
                OnWarriorPositionChanged.SC(fx, fy, tx, ty);
            }

            OnWarriorMovingOnPath.SC(warrior, movedPath);
        }

        // 执行攻击
        public event Action<Warrior, int, int> OnWarriorAttackAt = null; // 角色进行攻击
        public virtual void DoAttackAt(Warrior attacker, int tx, int ty)
        {
            Debug.Assert(attacker != null, "attacker should not be null");
            OnWarriorAttackAt.SC(attacker, tx, ty);
        }
    }
}
