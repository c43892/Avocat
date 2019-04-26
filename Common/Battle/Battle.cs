using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        // 战场内的 ai 列表
        public Dictionary<int, Dictionary<int, WarriorAI>> AIs { get; } = new Dictionary<int, Dictionary<int, WarriorAI>>();

        // 移除指定角色
        protected AsyncCalleeChain<Warrior> BeforeWarriorRemoved = new AsyncCalleeChain<Warrior>();
        protected AsyncCalleeChain<Warrior> AfterWarriorRemoved = new AsyncCalleeChain<Warrior>();
        public AsyncCalleeChain<Warrior> OnWarriorRemoved = new AsyncCalleeChain<Warrior>();
        public IEnumerator RemoveWarrior(Warrior warrior)
        {
            yield return BeforeWarriorRemoved.Invoke(warrior);

            // 移除角色前，先移除身上的 buff 效果和 AI
            AIs[warrior.Team].Remove(warrior.IDInMap);
            for (var i = 0; i < warrior.Buffs.Count; i++)
                yield return warrior.Buffs[i].OnDetached();

            Map.RemoveWarrior(warrior);

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
                    if (warrior.Team == 1)
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

        // 重置指定角色行动标记
        public AsyncCalleeChain<Warrior, Action<bool, bool>> BeforeResetActionFlag = new AsyncCalleeChain<Warrior, Action<bool, bool>>();
        public AsyncCalleeChain<Warrior> AfterResetActionFlag = new AsyncCalleeChain<Warrior>();
        public AsyncCalleeChain<Warrior> DoResetActionFlag = new AsyncCalleeChain<Warrior>();
        public IEnumerator ResetActionFlag(Warrior warrior)
        {
            var resetMovedFlag = true;
            var resetActionFlag = true;

            yield return BeforeResetActionFlag.Invoke(warrior, (_resetMovedFlag, _resetActionFlag) =>
            {
                resetMovedFlag = _resetMovedFlag;
                resetActionFlag = _resetActionFlag;
            });

            if (resetMovedFlag) warrior.Moved = false; // 重置行动标记
            if (resetActionFlag) warrior.ActionDone = false; // 重置行动标记

            yield return DoResetActionFlag.Invoke(warrior);
            yield return AfterResetActionFlag.Invoke(warrior);
        }

        // 回合开始，重置所有角色行动标记
        public AsyncCalleeChain<int> BeforeStartNextRound = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> AfterStartNextRound = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnNextRoundStarted = new AsyncCalleeChain<int>();
        public IEnumerator StartNextRound(int player)
        {
            yield return BeforeStartNextRound.Invoke(player);

            // 处理所属当前队伍的 ai
            if (AIs.ContainsKey(player))
                foreach (var id in AIs[player].Keys.ToArray())
                    yield return AIs[player][id].ActFirst?.Invoke();

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

                MoveWarroirs(warrior, tx, ty);

                fx = tx;
                fy = ty;
            }

            warrior.Moved = true;

            yield return OnWarriorMovingOnPath.Invoke(warrior, x, y, movedPath);
            yield return AfterMoveOnPath.Invoke(warrior, x, y, movedPath);
        }

        // 计算伤害
        public delegate void CalculateDamageAction<T1, T2, T3>(Warrior attacker, Warrior target, List<string> flags, out T1 inc, out T2 more, out T3 damageDec);
        public CalculateDamageAction<int, int, int> BeforeCalculateDamage1;
        public CalculateDamageAction<int, int, int> BeforeCalculateDamage2;
        public int CalculateDamage(Warrior attacker, Warrior target, List<string> flags)
        {
            // 物理和法术分别取不同的抗性，混乱攻击忽视抗性

            var basicAttackValue = attacker.BasicAttackValue;
            var inc = 0;
            var more = 0;
            var damageDecFac = 0; // 减伤系数

            if (flags.Contains("physic"))
            {
                inc = attacker.ATKInc;
                more = attacker.ATKMore;
                damageDecFac = target.ARM;
            }
            else if (flags.Contains("magic"))
            {
                inc = attacker.POWInc;
                more = attacker.POWMore;
                damageDecFac = target.RES;
            }

            // 通知所有可能影响抗性计算的逻辑
            BeforeCalculateDamage1?.Invoke(attacker, target, flags, out inc, out more, out damageDecFac);
            BeforeCalculateDamage2?.Invoke(attacker, target, flags, out inc, out more, out damageDecFac);

            // 计算最终攻击值
            var damage = basicAttackValue * (1 + inc) * (1 + more);
            damage = damage - damage * damageDecFac / (100 + damageDecFac);
            return damage < 0 ? 0 : (int)damage;
        }

        // 执行攻击
        public AsyncCalleeChain<Warrior, Warrior, List<string>> BeforeAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>();
        public AsyncCalleeChain<Warrior, Warrior, List<string>> AfterAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>();
        public AsyncCalleeChain<Warrior, Warrior, List<string>> OnWarriorAttack = new AsyncCalleeChain<Warrior, Warrior, List<string>>(); // 角色进行攻击
        public IEnumerator Attack(Warrior attacker, Warrior target, params string[] flags)
        {
            Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                yield break;

            // 整理攻击标记
            var attackFlags = new List<string>();
            attackFlags.AddRange(flags);
            attackFlags.Add(attacker.AttackingType);

            yield return BeforeAttack.Invoke(attacker, target, attackFlags);

            if (attackFlags.Contains("CancelAttack")) // 取消攻击标记
                yield break;

            // 计算实际伤害
            var damage = CalculateDamage(attacker, target, attackFlags);

            // ExtraAttack 不影响行动标记
            if (!attackFlags.Contains("ExtraAttack"))
                attacker.ActionDone = true;

            yield return OnWarriorAttack.Invoke(attacker, target, attackFlags);

            // 混乱攻击不计算护盾，其它类型攻击需要先消耗护盾
            var dhp = -damage;
            var des = 0;
            if (!attackFlags.Contains("chaos") && target.ES > 0)
            {
                dhp = damage > target.ES ? target.ES - damage : 0;
                des = damage > target.ES ? 0 : damage;
            }

            if (des != 0) yield return AddES(target, des);
            if (dhp != 0) yield return AddHP(target, dhp);

            yield return AfterAttack.Invoke(attacker, target, attackFlags);
        }

        // 角色变形
        public AsyncCalleeChain<Warrior, string> BeforeTransfrom = new AsyncCalleeChain<Warrior, string>();
        public AsyncCalleeChain<Warrior, string> AfterTransfrom = new AsyncCalleeChain<Warrior, string>();
        public AsyncCalleeChain<Warrior, string> OnTransfrom = new AsyncCalleeChain<Warrior, string>();
        public IEnumerator Transform(Warrior warrior, string state)
        {
            Debug.Assert(warrior is ITransformable, "the warrior is not transformable");

            yield return BeforeTransfrom.Invoke(warrior, state);

            (warrior as ITransformable).State = state;

            yield return OnTransfrom.Invoke(warrior, state);
            yield return AfterTransfrom.Invoke(warrior, state);
        }

        // 玩家本回合行动结束
        public AsyncCalleeChain<int> BeforeActionDone = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> AfterActionDone = new AsyncCalleeChain<int>();
        public AsyncCalleeChain<int> OnActionDone = new AsyncCalleeChain<int>();
        public IEnumerator ActionDone(int player)
        {
            yield return BeforeActionDone.Invoke(player);
            yield return OnActionDone.Invoke(player);

            // 处理所属当前队伍的 ai
            if (AIs.ContainsKey(player))
                foreach (var id in AIs[player].Keys.ToArray())
                    yield return AIs[player][id].ActLast?.Invoke();

            yield return AfterActionDone.Invoke(player);

            yield return TryBattleEnd(); // 回合结束时检查战斗结束条件
            yield return Move2NextPlayer(player); // 行动机会转移至下一队伍
        }

        // 添加角色到地图
        public AsyncCalleeChain<int, int, Warrior> BeforeAddWarrior = new AsyncCalleeChain<int, int, Warrior>();
        public AsyncCalleeChain<int, int, Warrior> AfterAddWarrior = new AsyncCalleeChain<int, int, Warrior>();
        public AsyncCalleeChain<int, int, Warrior> OnAddWarrior = new AsyncCalleeChain<int, int, Warrior>();
        public IEnumerator AddWarriorAt(int x, int y, Warrior warrior)
        {
            yield return BeforeAddWarrior.Invoke(x, y, warrior);

            Map.SetWarriorAt(x, y, warrior);

            if (warrior.AI != null)
                ResetWarriorAI(warrior);

            yield return OnAddWarrior.Invoke(x, y, warrior);
            yield return AfterAddWarrior.Invoke(x, y, warrior);
        }

        // 添加道具到地图
        public AsyncCalleeChain<int, int, BattleMapItem> BeforeAddItem = new AsyncCalleeChain<int, int, BattleMapItem>();
        public AsyncCalleeChain<int, int, BattleMapItem> AfterAddItem = new AsyncCalleeChain<int, int, BattleMapItem>();
        public AsyncCalleeChain<int, int, BattleMapItem> OnAddItem = new AsyncCalleeChain<int, int, BattleMapItem>();
        public IEnumerator AddItemAt(int x, int y, BattleMapItem item)
        {
            yield return BeforeAddItem.Invoke(x, y, item);

            Map.SetItemAt(x, y, item);

            yield return OnAddItem.Invoke(x, y, item);
            yield return AfterAddItem.Invoke(x, y, item);
        }

        // 重置角色 ai
        public void ResetWarriorAI(Warrior warrior)
        {
            if (!AIs.ContainsKey(warrior.Team))
                AIs[warrior.Team] = new Dictionary<int, WarriorAI>();

            AIs[warrior.Team][warrior.IDInMap] = warrior.AI;
        }

        #endregion

        #region 技能相关

        /// <summary>
        /// 添加 buff 或被动技能，如果目标对象为 null，则认为是场地效果
        /// </summary>
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
                buff.Warrior = target;
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

        /// <summary>
        /// 移除 buff 或被动技能效果
        /// </summary>
        protected AsyncCalleeChain<Buff, Warrior> BeforeBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        protected AsyncCalleeChain<Buff, Warrior> AfterBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        public AsyncCalleeChain<Buff, Warrior> OnBuffRemoved = new AsyncCalleeChain<Buff, Warrior>();
        public virtual IEnumerator RemoveBuff(Buff buff)
        {
            var target = buff.Warrior;

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
            buff.Warrior = null;
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
        public AsyncCalleeChain<Warrior> OnWarriorDying = new AsyncCalleeChain<Warrior>(); // 角色死亡
        public IEnumerator AddHP(Warrior warrior, int dhp)
        {
            yield return BeforeAddHP.Invoke(warrior, dhp, (int _dhp) => dhp = _dhp);

            warrior.HP = (warrior.HP + dhp).Clamp(0, warrior.MaxHP);

            yield return OnAddHP.Invoke(warrior, dhp);
            yield return AfterAddHP.Invoke(warrior, dhp);

            if (warrior.IsDead)
            {
                yield return OnWarriorDying.Invoke(warrior);
                yield return RemoveWarrior(warrior);
            }
        }

        // 角色加盾
        public AsyncCalleeChain<Warrior, int, Action<int>> BeforeAddES = new AsyncCalleeChain<Warrior, int, Action<int>>();
        public AsyncCalleeChain<Warrior, int> AfterAddES = new AsyncCalleeChain<Warrior, int>();
        public AsyncCalleeChain<Warrior, int> OnAddES = new AsyncCalleeChain<Warrior, int>();
        public IEnumerator AddES(Warrior warrior, int des)
        {
            yield return BeforeAddES.Invoke(warrior, des, (int _des) => des = _des);

            warrior.ES = (warrior.ES + des).Clamp(0, warrior.MaxES);

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