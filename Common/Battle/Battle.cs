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
    public abstract class Battle
    {
        // 由初始随机种子确定的伪随机序列
        public SRandom Srand { get; protected set; }

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
        protected AsyncCalleeChain<Warrior> BeforeWarriorRemoved = new AsyncCalleeChain<Warrior>();
        protected AsyncCalleeChain<Warrior> AfterWarriorRemoved = new AsyncCalleeChain<Warrior>();
        public AsyncCalleeChain<Warrior> OnWarriorRemoved = new AsyncCalleeChain<Warrior>();
        public IEnumerator RemoveWarrior(Warrior warrior)
        {
            yield return BeforeWarriorRemoved.Invoke(warrior);

            // 移除角色前，先移除身上的 buff 效果
            for (var i = 0; i < warrior.Buffs.Count; i++)
                yield return warrior.Buffs[i].OnDetached();

            warrior.GetPosInMap(out int x, out int y);
            Map.SetWarriorAt(x, y, null);
            warrior.Map = null;

            yield return OnWarriorRemoved.Invoke(warrior);
            yield return AfterWarriorRemoved.Invoke(warrior);
        }

        #region 战斗准备过程

        // 移动角色位置
        public void MoveWarroirs(Warrior warrior, int tx, int ty)
        {
            warrior.GetPosInMap(out int fx, out int fy);
            if (fx == tx && fy == ty)
                return;

            Debug.Assert(!map.BlockedAt(tx, ty), "target position has been blocked: " + tx + ", " + ty);

            Map.SetWarriorAt(fx, fy, null);
            Map.SetWarriorAt(tx, ty, warrior);
        }

        // 交换英雄位置
        protected AsyncCalleeChain<int, int, int, int> BeforeExchangeWarroirsPosition = new AsyncCalleeChain<int, int, int, int>();
        protected AsyncCalleeChain<int, int, int, int> AfterExchangeWarroirsPosition = new AsyncCalleeChain<int, int, int, int>();
        public AsyncCalleeChain<int, int, int, int> OnWarriorPositionExchanged = new AsyncCalleeChain<int, int, int, int>();
        public IEnumerator ExchangeWarroirsPosition(int fx, int fy, int tx, int ty)
        {
            if (fx == tx && fy == ty)
                yield break;

            Debug.Assert(!map.BlockedAt(tx, ty), "target position has been blocked: " + tx + ", " + ty);

            yield return BeforeExchangeWarroirsPosition.Invoke(fx, fy, tx, ty);

            var tmp = Map.GetWarriorAt(fx, fy);
            Map.SetWarriorAt(fx, fy, Map.GetWarriorAt(tx, ty));
            Map.SetWarriorAt(tx, ty, tmp);

            yield return OnWarriorPositionExchanged.Invoke(fx, fy, tx, ty);
            yield return AfterExchangeWarroirsPosition.Invoke(fx, fy, tx, ty);
        }

        // 完成战斗准备
        protected AsyncCalleeChain<int> BeforePlayerPrepared = new AsyncCalleeChain<int>();
        protected AsyncCalleeChain<int> AfterPlayerPrepared = new AsyncCalleeChain<int>();
        public abstract void PlayerPreparedImpl(int player);
        public AsyncCalleeChain<int> OnPlayerPrepared = new AsyncCalleeChain<int>(); // 有玩家完成战斗准备
        public IEnumerator PlayerPrepared(int player)
        {
            yield return BeforePlayerPrepared.Invoke(player);

            PlayerPreparedImpl(player);

            yield return AfterPlayerPrepared.Invoke(player);

            yield return OnPlayerPrepared.Invoke(player);
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
        public AsyncCalleeChain<int> BeforeStartNextRound = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> AfterStartNextRound = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnNextRoundStarted = new AsyncCalleeChain<int>();
        public IEnumerator StartNextRound(int player)
        {
            yield return BeforeStartNextRound.Invoke(player);

            Map.ForeachWarriors((i, j, warrior) =>
            {
                warrior.Moved = false;
                warrior.ActionDone = false;
            });

            yield return OnNextRoundStarted.Invoke(player);
            yield return AfterStartNextRound.Invoke(player);
        }

        // 角色沿路径移动
        public AsyncCalleeChain<Warrior> BeforeMoveOnPath = new AsyncCalleeChain<Warrior>();
        public AsyncCalleeChain<Warrior, int, int, List<int>> AfterMoveOnPath = new AsyncCalleeChain<Warrior, int, int, List<int>>();
        public AsyncCalleeChain<Warrior, int, int, List<int>> OnWarriorMovingOnPath = new AsyncCalleeChain<Warrior, int, int, List<int>>(); // 角色沿路径移动
        public IEnumerator MoveOnPath(Warrior warrior)
        {
            Debug.Assert(!warrior.Moved, "attacker has already moved in this round");

            warrior.GetPosInMap(out int x, out int y);
            Debug.Assert(MU.ManhattanDist(x, y, warrior.MovingPath[0], warrior.MovingPath[1]) <= 1, "the warrior has not been right on the start position: " + x + ", " + y);

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

                // Debug.Assert(!map.BlockedAt(tx, ty), "target position has been blocked: " + tx + ", " + ty);

                MoveWarroirs(warrior, tx, ty);

                fx = tx;
                fy = ty;
            }

            warrior.Moved = true;

            yield return OnWarriorMovingOnPath.Invoke(warrior, x, y, movedPath);
            yield return AfterMoveOnPath.Invoke(warrior, x, y, movedPath);
        }

        // 执行攻击
        public AsyncCalleeChain<Warrior, Warrior, List<string>> BeforeAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>();
        public AsyncCalleeChain<Warrior, Warrior, List<string>> AfterAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>();
        public AsyncCalleeChain<Warrior, Warrior, List<string>> OnWarriorAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>(); // 角色进行攻击
        public AsyncCalleeChain<Warrior> OnWarriorDying = new AsyncCalleeChain<Warrior>(); // 角色死亡
        public IEnumerator Attack(Warrior attacker, Warrior target, params string[] flags)
        {
            Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                yield break;

            var attackFlags = new List<string>();
            attackFlags.AddRange(flags);

            yield return BeforeAttack.Invoke(attacker, target, attackFlags);

            target.ES -= attacker.ATK;
            if (target.ES < 0)
            {
                target.HP += target.ES;
                target.ES = 0;
            }

            // extraAttack 不影响行动标记
            if (!attackFlags.Contains("extraAttack"))
                attacker.ActionDone = true;

            yield return OnWarriorAttack.Invoke(attacker, target, attackFlags);
            yield return AfterAttack.Invoke(attacker, target, attackFlags);

            if (target.IsDead)
            {
                yield return OnWarriorDying.Invoke(target);
                yield return RemoveWarrior(target);
            }
        }

        // 玩家本回合行动结束
        public AsyncCalleeChain<int> BeforeActionDone = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> AfterActionDone = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnActionDone = new AsyncCalleeChain<int>();
        public IEnumerator ActionDone(int player)
        {
            yield return BeforeActionDone.Invoke(player);
            yield return OnActionDone.Invoke(player);
            yield return AfterActionDone.Invoke(player);

            yield return TryBattleEnd(); // 回合结束时检查战斗结束条件
            yield return Move2NextPlayer(player); // 行动机会转移至玩家开始行动
        }

        #endregion

        #region 技能相关

        protected List<Buff> GroundBuffs = new List<Buff>();
        protected AsyncCalleeChain<Buff, Warrior> BeforeBuffAttached = new AsyncCalleeChain<Buff, Warrior>();
        protected AsyncCalleeChain<Buff, Warrior> AfterBuffAttached = new AsyncCalleeChain<Buff, Warrior>();
        public AsyncCalleeChain<Buff, Warrior> OnBuffAttached = new AsyncCalleeChain<Buff, Warrior>();
        public virtual IEnumerator AddBuff(Buff buff, Warrior target = null)
        {
            yield return BeforeBuffAttached.Invoke(buff, target);

            if (target != null)
            {
                Debug.Assert(!target.Buffs.Contains(buff), "buff " + buff.Name + " already attached to target (" + target.AvatarID + "," + target.IDInMap + ")");
                target.Buffs.Add(buff);
                buff.Owner = target;
            }
            else
            {
                Debug.Assert(!GroundBuffs.Contains(buff), "buff " + buff.Name + " already attached to ground");
                GroundBuffs.Add(buff);
            }

            buff.Battle = this;
            yield return buff.OnAttached();

            yield return OnBuffAttached.Invoke(buff, target);
            yield return AfterBuffAttached.Invoke(buff, target);
        }

        protected AsyncCalleeChain<Buff, Warrior> BeforeBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        protected AsyncCalleeChain<Buff, Warrior> AfterBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        public AsyncCalleeChain<Buff, Warrior> OnBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        public virtual IEnumerator RemoveBuff(Buff buff)
        {
            var target = buff.Owner;

            yield return BeforeBuffRemoved.Invoke(buff, target);

            if (target != null)
            {
                Debug.Assert(target.Buffs.Contains(buff), "buff " + buff.Name + " has not been attached to target (" + target.AvatarID + "," + target.IDInMap + ")");
                target.Buffs.Remove(buff);
            }
            else
            {
                Debug.Assert(GroundBuffs.Contains(buff), "buff " + buff.Name + " has not been attached to ground)");
                GroundBuffs.Remove(buff);
            }

            yield return buff.OnDetached();
            buff.Owner = null;
            buff.Battle = null;

            yield return OnBuffRemoved.Invoke(buff, target);
            yield return AfterBuffRemoved.Invoke(buff, target);
        }

        #endregion

        #region 战斗基本流程

        int[] players = null; // 初始的玩家轮转列表
        List<int> playerSeq = new List<int>();  // 当前剩余轮转

        // 所有玩家完成战斗准备，自动开始新回合
        IEnumerator OnAfterPlayerPrepared(int player)
        {
            if (AllPrepared)
                yield return StartNextRound(playerSeq[0]);
        }

        // 多玩家行动机会的轮转
        void BattleStatusTransfer(params int[] players)
        {
            this.players = players;
            playerSeq.AddRange(players);

            AfterPlayerPrepared.Add(OnAfterPlayerPrepared);
        }

        // 行动机会转移至玩家开始行动
        public virtual IEnumerator Move2NextPlayer(int lastPlayer)
        {
            // 行动机会轮转至下一玩家
            Debug.Assert(lastPlayer == playerSeq[0]);
            playerSeq.RemoveAt(0);
            if (playerSeq.Count == 0)
                playerSeq.AddRange(players);

            yield return StartNextRound(playerSeq[0]);
        }

        // 结束战斗
        public AsyncCalleeChain<int> OnBattleEnded = new AsyncCalleeChain<int>(); // 战斗结束通知
        public IEnumerator TryBattleEnd()
        {
            var r = CheckEndCondition();
            if (r != 0)
                yield return OnBattleEnded.Invoke(r);
        }

        // 构建基本逻辑，参数表示玩家轮转编号列表
        protected virtual Battle Build(params int[] players)
        {
            BattleStatusTransfer(players);
            FC.Async2Sync(AddBuff(new ResetES())); // 回合开始时重置护盾
            FC.Async2Sync(AddBuff(new ResetActionFlag())); // 回合开始时重置行动标记
            return this;
        }

        #endregion

        #region 角色操作包装

        // 角色加血
        public AsyncCalleeChain<Warrior, int, Action<int>> BeforeAddHP = new AsyncCalleeChain<Warrior, int, Action<int>>();
        public AsyncCalleeChain<Warrior, int> AfterAddHP = new AsyncCalleeChain<Warrior, int>();
        public AsyncCalleeChain<Warrior, int> OnAddHP = new AsyncCalleeChain<Warrior, int>();
        public IEnumerator AddHP(Warrior warrior, int dhp)
        {
            yield return BeforeAddHP.Invoke(warrior, dhp, (int _dhp) => dhp = _dhp);

            warrior.HP = MU.Clamp(warrior.HP + dhp, 0, warrior.MaxHP);

            yield return OnAddHP.Invoke(warrior, dhp);
            yield return AfterAddHP.Invoke(warrior, dhp);
        }

        // 角色加盾
        public AsyncCalleeChain<Warrior, int, Action<int>> BeforeAddES = new AsyncCalleeChain<Warrior, int, Action<int>>();
        public AsyncCalleeChain<Warrior, int> AfterAddES = new AsyncCalleeChain<Warrior, int>();
        public AsyncCalleeChain<Warrior, int> OnAddES = new AsyncCalleeChain<Warrior, int>();
        public IEnumerator AddES(Warrior warrior, int des)
        {
            yield return BeforeAddES.Invoke(warrior, des, (int _des) => des = _des);

            warrior.ES = MU.Clamp(warrior.ES + des, 0, warrior.MaxES);

            yield return OnAddES.Invoke(warrior, des);
            yield return AfterAddES.Invoke(warrior, des);
        }

        // 角色加攻
        public AsyncCalleeChain<Warrior, int, Action<int>> BeforeAddATK = new AsyncCalleeChain<Warrior, int, Action<int>>();
        public AsyncCalleeChain<Warrior, int> AfterAddATK = new AsyncCalleeChain<Warrior, int>();
        public AsyncCalleeChain<Warrior, int> OnAddATK = new AsyncCalleeChain<Warrior, int>();
        public IEnumerator AddATK(Warrior warrior, int atk)
        {
            yield return BeforeAddATK.Invoke(warrior, atk, (int _atk) => atk = _atk);

            warrior.ATK += atk;
            if (warrior.ATK < 0)
                warrior.ATK = 0;

            yield return OnAddATK.Invoke(warrior, atk);
            yield return AfterAddATK.Invoke(warrior, atk);
        }

        #endregion
    }
}