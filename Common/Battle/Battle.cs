using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗对象，包括一场战斗所需全部数据，在初始数据和操作过程完全一致的情况下，可完全复现一场战斗过程。
    /// 但战斗对象本身不包括战斗驱动过程，而是有 BattleRoom 驱动。
    /// </summary>
    public abstract partial class Battle
    {
        // 由初始随机种子确定的伪随机序列
        protected SRandom Srand { get; set; }

        // 战斗地图
        public BattleMap Map
        {
            get
            {
                return map;
            }
            private set
            {
                if (map != null)
                    map.Battle = null;

                map = value;
                map.Battle = this;
    }
        } BattleMap map = null;

        public Battle(BattleMap map, int randSeed)
        {
            Srand = new SRandom(randSeed);
            Map = map;
        }

        // 移除指定角色
        public void RemoveWarrior(Warrior warrior)
        {
            warrior.GetPosInMap(out int x, out int y);
            Map.SetWarriorAt(x, y, null);
            warrior.Map = null;
        }

        #region 战斗准备过程

        // 交换英雄位置
        protected AsyncCalleeChain<int, int, int, int> BeforeExchangeWarroirsPosition = new AsyncCalleeChain<int, int, int, int>();
        protected AsyncCalleeChain<int, int, int, int> AfterExchangeWarroirsPosition = new AsyncCalleeChain<int, int, int, int>();
        public AsyncCalleeChain<int, int, int, int> OnWarriorPositionExchanged = new AsyncCalleeChain<int, int, int, int>();
        public IEnumerator ExchangeWarroirsPosition(int fx, int fy, int tx, int ty, bool suppressPositionExchangedEvent = false)
        {
            if (fx == tx && fy == ty)
                yield break;

            Debug.Assert(!map.BlockedAt(tx, ty), "target position has been blocked: " + tx + ", " + ty);

            yield return BeforeExchangeWarroirsPosition.Invoke(fx, fy, tx, ty);

            var tmp = Map.GetWarriorAt(fx, fy);
            Map.SetWarriorAt(fx, fy, Map.GetWarriorAt(tx, ty));
            Map.SetWarriorAt(tx, ty, tmp);

            yield return AfterExchangeWarroirsPosition.Invoke(fx, fy, tx, ty);

            if (!suppressPositionExchangedEvent)
                yield return OnWarriorPositionExchanged.Invoke(fx, fy, tx, ty);
        }

        // 完成战斗准备
        protected AsyncCalleeChain<int> BeforePlayerPrepared = new AsyncCalleeChain<int>();
        protected AsyncCalleeChain<int> AfterPlayerPrepared = new AsyncCalleeChain<int>();
        public abstract void PlayerPreparedImpl(int player);
        public AsyncCalleeChain<int> OnPlayerPrepared = new AsyncCalleeChain<int>(); // 有玩家完成战斗准备
        public IEnumerator PlayerPrepared(int player)
        {
            yield return BeforePlayerPrepared?.Invoke(player);

            PlayerPreparedImpl(player);

            yield return AfterPlayerPrepared?.Invoke(player);

            yield return OnPlayerPrepared?.Invoke(player);
        }

        // 检查所有玩家是否都已经完成战斗准备
        public abstract bool AllPrepared { get; }

        // 检查战斗结束条件，0 表示尚未结束，否则返回值表示胜利玩家
        // 检查战斗结束条件
        public virtual int CheckEndCondition()
        {
            // 双方至少各存活一个角色
            var team1Survived = false;
            var team2Survived = false;
            FC.For2(Map.Width, Map.Height, (x, y) =>
            {
                var warrior = Map.GetWarriorAt(x, y);
                if (warrior != null && !warrior.IsDead)
                {
                    if (warrior.Owner == 1)
                        team1Survived = true;
                    else
                        team2Survived = true;
                }
            }, () => !team1Survived || !team2Survived);

            if (team1Survived && !team2Survived)
                return 1;
            else if (team2Survived && !team1Survived)
                return 2;
            else
                return 0;
        }

        #endregion

        #region 战斗过程

        // 回合开始，重置所有角色行动标记
        protected AsyncCalleeChain<int> BeforeStartNextRound = new AsyncCalleeChain<int>();
        protected AsyncCalleeChain<int> AfterStartNextRound = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnNextRoundStarted = new AsyncCalleeChain<int>();
        public IEnumerator StartNextRound(int player)
        {
            yield return BeforeStartNextRound.Invoke(player);

            Map.ForeachWarriors((i, j, warrior) =>
            {
                warrior.Moved = false;
                warrior.ActionDone = false;
            });

            yield return AfterStartNextRound?.Invoke(player);

            yield return OnNextRoundStarted?.Invoke(player);
        }

        // 角色沿路径移动
        protected AsyncCalleeChain<Warrior> BeforeMoveOnPath = new AsyncCalleeChain<Warrior>();
        protected AsyncCalleeChain<Warrior, int, int, List<int>> AfterMoveOnPath = new AsyncCalleeChain<Warrior, int, int, List<int>>();
        public AsyncCalleeChain<Warrior, int, int, List<int>> OnWarriorMovingOnPath = new AsyncCalleeChain<Warrior, int, int, List<int>>(); // 角色沿路径移动
        public IEnumerator MoveOnPath(Warrior warrior)
        {
            Debug.Assert(!warrior.Moved, "attacker has already moved in this round");

            warrior.GetPosInMap(out int x, out int y);
            Debug.Assert(MU.ManhattanDist(x, y, warrior.MovingPath[0], warrior.MovingPath[1]) == 1, "the warrior has not been right on the start position: " + x + ", " + y);

            if (warrior.MovingPath.Count > warrior.MoveRange * 2) // 超出移动能力
                yield break;

            yield return BeforeMoveOnPath.Invoke(warrior);

            List<int> movedPath = new List<int>(); // 实际落实了的移动路径
            var lstPathXY = warrior.MovingPath;
            var fx = x;
            var fy = y;

            while (lstPathXY.Count >= 2)
            {
                var tx = lstPathXY[0];
                var ty = lstPathXY[1];

                lstPathXY.RemoveRange(0, 2);
                movedPath.Add(tx);
                movedPath.Add(ty);

                Debug.Assert(!map.BlockedAt(tx, ty), "target position has been blocked: " + tx + ", " + ty);

                yield return ExchangeWarroirsPosition(fx, fy, tx, ty, true);

                fx = tx;
                fy = ty;
            }

            warrior.Moved = true;

            yield return AfterMoveOnPath.Invoke(warrior, x, y, movedPath);
            yield return OnWarriorMovingOnPath.Invoke(warrior, x, y, movedPath);
        }

        // 执行攻击
        protected AsyncCalleeChain<Warrior, Warrior> BeforeAttack = new AsyncCalleeChain<Warrior, Warrior>();
        protected AsyncCalleeChain<Warrior, Warrior> AfterAttack = new AsyncCalleeChain<Warrior, Warrior>();
        public AsyncCalleeChain<Warrior, Warrior> OnWarriorAttack = new AsyncCalleeChain<Warrior, Warrior>(); // 角色进行攻击
        public AsyncCalleeChain<Warrior> OnWarriorDying = new AsyncCalleeChain<Warrior>(); // 角色死亡
        public IEnumerator Attack(Warrior attacker, Warrior target)
        {
            Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                yield break;

            yield return BeforeAttack.Invoke(attacker, target);

            target.Shield -= attacker.Power;
            if (target.Shield < 0)
            {
                target.Hp += target.Shield;
                target.Shield = 0;
            }

            attacker.ActionDone = true;

            yield return AfterAttack.Invoke(attacker, target);

            yield return OnWarriorAttack.Invoke(attacker, target);

            if (target.IsDead)
            {
                yield return OnWarriorDying.Invoke(target);
                RemoveWarrior(target);
            }
        }

        // 玩家本回合行动结束
        protected AsyncCalleeChain<int> BeforeActionDone = new AsyncCalleeChain<int>();
        protected AsyncCalleeChain<int> AfterActionDone = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnActionDone = new AsyncCalleeChain<int>();
        public IEnumerator ActionDone(int player)
        {
            yield return BeforeActionDone.Invoke(player);
            yield return AfterActionDone.Invoke(player);
            yield return OnActionDone.Invoke(player);
        }

        #endregion
    }
}