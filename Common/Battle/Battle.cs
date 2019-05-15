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

        // 本场游戏录像
        public BattleReplay Replay { get; } = new BattleReplay { Time = DateTime.Now.Ticks };

        public Battle(BattleMap map, int randSeed)
        {
            Srand = new SRandom(randSeed);
            Map = map;
        }

        // 战场内的 ai 列表
        public Dictionary<int, Dictionary<int, WarriorAI>> AIs { get; } = new Dictionary<int, Dictionary<int, WarriorAI>>();

        // 移除指定角色
        public event Action<Warrior> BeforeWarriorRemoved = null;
        public Action<Warrior> AfterWarriorRemoved = null;
        public Action<Warrior> OnWarriorRemoved = null;
        public void RemoveWarrior(Warrior warrior)
        {
            BeforeWarriorRemoved?.Invoke(warrior);

            // 移除角色前，先移除身上的 buff 效果和 AI

            if (AIs.ContainsKey(warrior.Team))
                AIs[warrior.Team].Remove(warrior.IDInMap);

            for (var i = 0; i < warrior.Buffs.Count; i++)
                warrior.Buffs[i].OnDetached();

            Map.RemoveWarrior(warrior);

            OnWarriorRemoved?.Invoke(warrior);
            AfterWarriorRemoved?.Invoke(warrior);
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
        public event Action<int, int, int, int> BeforeExchangeWarroirsPosition = null;
        public event Action<int, int, int, int> AfterExchangeWarroirsPosition = null;
        public event Action<int, int, int, int> OnWarriorPositionExchanged = null;
        public void ExchangeWarroirsPosition(int fx, int fy, int tx, int ty)
        {
            if (fx == tx && fy == ty)
                return;

            BeforeExchangeWarroirsPosition?.Invoke(fx, fy, tx, ty);

            var tmp = Map.GetWarriorAt(fx, fy);
            Map.SetWarriorAt(fx, fy, Map.GetWarriorAt(tx, ty));
            Map.SetWarriorAt(tx, ty, tmp);

            OnWarriorPositionExchanged?.Invoke(fx, fy, tx, ty);
            AfterExchangeWarroirsPosition?.Invoke(fx, fy, tx, ty);
        }

        // 玩家完成战斗准备
        public abstract void PlayerPreparedImpl(int player);
        public abstract bool AllPrepared { get; }

        // 完成战斗准备
        protected event Action<int> BeforePlayerPrepared = null;
        protected event Action<int> AfterPlayerPrepared = null;
        public event Action<int> OnPlayerPrepared = null; // 有玩家完成战斗准备
        public void PlayerPrepared(int player)
        {
            BeforePlayerPrepared?.Invoke(player);
            PlayerPreparedImpl(player);
            AfterPlayerPrepared?.Invoke(player);
            OnPlayerPrepared?.Invoke(player);
        }

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
        public event Action<Warrior, Action<bool, bool>> BeforeResetActionFlag = null;
        public event Action<Warrior> AfterResetActionFlag = null;
        public event Action<Warrior> DoResetActionFlag = null;
        public void ResetActionFlag(Warrior warrior)
        {
            var resetMovedFlag = true;
            var resetActionFlag = true;

            BeforeResetActionFlag?.Invoke(warrior, (_resetMovedFlag, _resetActionFlag) =>
            {
                resetMovedFlag = _resetMovedFlag;
                resetActionFlag = _resetActionFlag;
            });

            if (resetMovedFlag) warrior.Moved = false; // 重置行动标记
            if (resetActionFlag) SetActionFlag(warrior, false); // 重置行动标记

            DoResetActionFlag?.Invoke(warrior);
            AfterResetActionFlag?.Invoke(warrior);
        }

        // 回合开始，重置所有角色行动标记
        public event Action<int> BeforeStartNextRound = null;
        public event Action<int> AfterStartNextRound = null;
        public event Action<int> OnNextRoundStarted = null;
        public void StartNextRound(int player)
        {
            BeforeStartNextRound?.Invoke(player);

            // 处理所属当前队伍的 ai
            if (AIs.ContainsKey(player))
            {
                var ais = AIs[player].Values.ToArray();
                ais.Resort();
                foreach (var id in AIs[player].Keys.ToArray())
                    AIs[player][id].ActFirst?.Invoke();
            }
                
            OnNextRoundStarted?.Invoke(player);
            AfterStartNextRound?.Invoke(player);
        }

        // 角色沿路径移动
        public event Action<Warrior> BeforeMoveOnPath = null;
        public event Action<Warrior, int, int, List<int>> AfterMoveOnPath = null;
        public event Action<Warrior, int, int, List<int>> OnWarriorMovingOnPath = null; // 角色沿路径移动
        public void MoveOnPath(Warrior warrior)
        {
            Debug.Assert(!warrior.Moved, "attacker has already moved in this round");

            warrior.GetPosInMap(out int x, out int y);
            Debug.Assert(MU.ManhattanDist(x, y, warrior.MovingPath[0], warrior.MovingPath[1]) == 1, "the warrior has not been right on the start position: " + x + ", " + y);

            if (warrior.MovingPath.Count > warrior.MoveRange * 2) // 超出移动能力
                return;

            BeforeMoveOnPath?.Invoke(warrior);

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

            OnWarriorMovingOnPath?.Invoke(warrior, x, y, movedPath);
            AfterMoveOnPath?.Invoke(warrior, x, y, movedPath);
        }

        // 计算伤害
        public delegate void CalculateDamageAction(Warrior attacker, Warrior target, List<string> flags, out int inc, out int more, out int damageDec);
        public event CalculateDamageAction BeforeCalculateDamage1;
        public event CalculateDamageAction BeforeCalculateDamage2;
        public int CalculateDamage(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            // 物理和法术分别取不同的抗性，混乱攻击忽视抗性

            var basicAttack = attacker.BasicAttackValue;
            if (skill is ISkillWithAXY)
                basicAttack = Calculation.CalcBasicAttackByAXY(attacker, skill as ISkillWithAXY);

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

            // 通知所有可能影响各种系数的计算逻辑
            BeforeCalculateDamage1?.Invoke(attacker, target, flags, out inc, out more, out damageDecFac);
            BeforeCalculateDamage2?.Invoke(attacker, target, flags, out inc, out more, out damageDecFac);

            // 计算最终攻击值
            return Calculation.CalcDamage(basicAttack, inc, more, damageDecFac);
        }

        // 变更行动标记
        public event Action<Warrior, bool> BeforeSetActionFlag = null;
        public event Action<Warrior, bool> AfterSetActionFlag = null;
        public event Action<Warrior, bool> OnSetActionFlag = null;
        public void SetActionFlag(Warrior warrior, bool acted)
        {
            BeforeSetActionFlag?.Invoke(warrior, acted);
            warrior.ActionDone = acted;
            OnSetActionFlag?.Invoke(warrior, acted);
            AfterSetActionFlag?.Invoke(warrior, acted);
        }

        // 执行攻击
        public event Action<Warrior, Warrior, Skill, List<string>> BeforeAttack = null;
        public event Action<Warrior, Warrior, Skill, List<string>> AfterAttack = null;
        public event Action<Warrior, Warrior, Skill, List<string>> OnWarriorAttack = null; // 角色进行攻击
        public void Attack(Warrior attacker, Warrior target, Skill skill = null, params string[] flags)
        {
            Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");

            // 在连续指令的情况下，可能条件已经不满足
            if (attacker == null || target == null)
                return;

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                return;

            // 整理攻击标记
            var attackFlags = new List<string>();
            attackFlags.AddRange(flags);
            attackFlags.Add(attacker.AttackingType);

            BeforeAttack?.Invoke(attacker, target, skill, attackFlags);

            if (attackFlags.Contains("CancelAttack")) // 取消攻击
                return;

            // 计算实际伤害
            var damage = CalculateDamage(attacker, target, skill, attackFlags);

            // ExtraAttack 不影响行动标记
            if (!attackFlags.Contains("ExtraAttack"))
                SetActionFlag(attacker, true);

            OnWarriorAttack?.Invoke(attacker, target, skill, attackFlags);

            // 混乱攻击不计算护盾，其它类型攻击需要先消耗护盾
            var dhp = -damage;
            var des = 0;
            if (!attackFlags.Contains("chaos") && target.ES > 0)
            {
                dhp = damage > target.ES ? target.ES - damage : 0;
                des = damage > target.ES ? 0 : -damage;
            }

            if (des != 0) AddES(target, des);
            if (dhp != 0) AddHP(target, dhp);

            AfterAttack?.Invoke(attacker, target, skill, attackFlags);
        }

        // 模拟攻击行为的伤害数值，但并不执行攻击行为
        public void SimulateAttackingDamage(Warrior attacker, Warrior target, Skill skill, Action<int> onDamage, params string[] flags)
        {
            Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");

            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                return;

            // 整理攻击标记
            var attackFlags = new List<string>();
            attackFlags.AddRange(flags);
            attackFlags.Add(attacker.AttackingType);

            BeforeAttack?.Invoke(attacker, target, skill, attackFlags);

            if (attackFlags.Contains("CancelAttack")) // 取消攻击
                return;

            // 计算实际伤害
            var damage = CalculateDamage(attacker, target, skill, attackFlags);
            onDamage(damage);
        }

        // 角色变形
        public event Action<Warrior, string> BeforeTransfrom = null;
        public event Action<Warrior, string> AfterTransfrom = null;
        public event Action<Warrior, string> OnTransfrom = null;
        public void Transform(Warrior warrior, string state)
        {
            Debug.Assert(warrior is ITransformable, "the warrior is not transformable");

            BeforeTransfrom?.Invoke(warrior, state);

            (warrior as ITransformable).State = state;

            OnTransfrom?.Invoke(warrior, state);
            AfterTransfrom?.Invoke(warrior, state);
        }

        // 玩家本回合行动结束
        public event Action<int> BeforeActionDone = null;
        public event Action<int> AfterActionDone = null;
        public event Action<int> OnActionDone = null;
        public void ActionDone(int player)
        {
            BeforeActionDone?.Invoke(player);
            OnActionDone?.Invoke(player);

            // 处理所属当前队伍的 ai
            if (AIs.ContainsKey(player))
            {
                var ais = AIs[player].Values.ToArray();
                ais.Resort();
                foreach (var id in AIs[player].Keys.ToArray())
                     AIs[player][id].ActLast?.Invoke();
            }

            AfterActionDone?.Invoke(player);

            TryBattleEnd(); // 回合结束时检查战斗结束条件
            Move2NextPlayer(player); // 行动机会转移至下一队伍
        }

        // 添加角色到地图
        public event Action<int, int, Warrior> BeforeAddWarrior = null;
        public event Action<int, int, Warrior> AfterAddWarrior = null;
        public event Action<int, int, Warrior> OnAddWarrior = null;
        public void AddWarriorAt(int x, int y, Warrior warrior)
        {
            BeforeAddWarrior?.Invoke(x, y, warrior);

            Map.SetWarriorAt(x, y, warrior);

            if (warrior.AI != null)
                ResetWarriorAI(warrior);

            OnAddWarrior?.Invoke(x, y, warrior);
            AfterAddWarrior?.Invoke(x, y, warrior);
        }

        // 添加道具到地图
        public event Action<int, int, BattleMapItem> BeforeAddItem = null;
        public event Action<int, int, BattleMapItem> AfterAddItem = null;
        public event Action<int, int, BattleMapItem> OnAddItem = null;
        public void AddItemAt(int x, int y, BattleMapItem item)
        {
            BeforeAddItem?.Invoke(x, y, item);

            Map.SetItemAt(x, y, item);

            OnAddItem?.Invoke(x, y, item);
            AfterAddItem?.Invoke(x, y, item);
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
        protected event Action<Buff, Warrior> BeforeBuffAttached = null;
        protected event Action<Buff, Warrior> AfterBuffAttached = null;
        public event Action<Buff, Warrior> OnBuffAttached = null;
        public virtual void AddBuff(Buff buff, Warrior target = null)
        {
            BeforeBuffAttached?.Invoke(buff, target);

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
            buff.OnAttached();

            OnBuffAttached?.Invoke(buff, target);
            AfterBuffAttached?.Invoke(buff, target);
        }

        /// <summary>
        /// 移除 buff 或被动技能效果
        /// </summary>
        protected event Action<Buff, Warrior> BeforeBuffRemoved = null;
        protected event Action<Buff, Warrior> AfterBuffRemoved = null;
        public event Action<Buff, Warrior> OnBuffRemoved = null;
        public virtual void RemoveBuff(Buff buff)
        {
            var target = buff.Warrior;

            BeforeBuffRemoved?.Invoke(buff, target);

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

            buff.OnDetached();
            buff.Warrior = null;
            buff.Battle = null;

            OnBuffRemoved?.Invoke(buff, target);
            AfterBuffRemoved?.Invoke(buff, target);
        }

        // 促发时光倒流
        public event Action<BattleReplay> OnTimeBackTriggered = null;
        public void TriggerTimeBack()
        {
            OnTimeBackTriggered(Replay);
        }

        #endregion

        #region 战斗基本流程

        int[] players = null; // 初始的玩家轮转列表
        List<int> playerSeq = new List<int>();  // 当前剩余轮转

        // 所有玩家完成战斗准备，自动开始新回合
        void OnAfterPlayerPrepared(int player)
        {
            if (AllPrepared)
                StartNextRound(playerSeq[0]);
        }

        // 多玩家行动机会的轮转
        void BattleStatusTransfer(params int[] players)
        {
            this.players = players;
            playerSeq.AddRange(players);

            AfterPlayerPrepared += OnAfterPlayerPrepared;
        }

        // 行动机会转移至玩家开始行动
        public virtual void Move2NextPlayer(int lastPlayer)
        {
            // 行动机会轮转至下一玩家
            Debug.Assert(lastPlayer == playerSeq[0]);
            playerSeq.RemoveAt(0);
            if (playerSeq.Count == 0)
                playerSeq.AddRange(players);

            StartNextRound(playerSeq[0]);
        }

        // 结束战斗
        public event Action<int> OnBattleEnded = null; // 战斗结束通知
        public void TryBattleEnd()
        {
            var r = CheckEndCondition();
            if (r != 0)
                OnBattleEnded.Invoke(r);
        }

        // 构建基本逻辑，参数表示玩家轮转编号列表
        protected virtual Battle Build(params int[] players)
        {
            BattleStatusTransfer(players);
            AddBuff(new ResetES()); // 回合开始时重置护盾
            AddBuff(new ResetActionFlag()); // 回合开始时重置行动标记
            return this;
        }

        #endregion

        #region 角色操作包装

        // 角色加血
        public event Action<Warrior, int, Action<int>> BeforeAddHP = null;
        public event Action<Warrior, int> AfterAddHP = null;
        public event Action<Warrior, int> OnAddHP = null;
        public event Action<Warrior> OnWarriorDying = null; // 角色死亡
        public void AddHP(Warrior warrior, int dhp)
        {
            BeforeAddHP?.Invoke(warrior, dhp, (int _dhp) => dhp = _dhp);

            warrior.HP = (warrior.HP + dhp).Clamp(0, warrior.MaxHP);

            OnAddHP?.Invoke(warrior, dhp);
            AfterAddHP?.Invoke(warrior, dhp);

            if (warrior.IsDead)
            {
                OnWarriorDying?.Invoke(warrior);
                RemoveWarrior(warrior);
            }
        }

        // 角色加盾
        public event Action<Warrior, int, Action<int>> BeforeAddES = null;
        public event Action<Warrior, int> AfterAddES = null;
        public event Action<Warrior, int> OnAddES = null;
        public void AddES(Warrior warrior, int des)
        {
            BeforeAddES?.Invoke(warrior, des, (int _des) => des = _des);

            warrior.ES = (warrior.ES + des).Clamp(0, warrior.MaxES);

            OnAddES?.Invoke(warrior, des);
            AfterAddES?.Invoke(warrior, des);
        }

        // 角色加攻
        public event Action<Warrior, int, Action<int>> BeforeAddATK = null;
        public event Action<Warrior, int> AfterAddATK = null;
        public event Action<Warrior, int> OnAddATK = null;
        public void AddATK(Warrior warrior, int atk)
        {
            BeforeAddATK?.Invoke(warrior, atk, (int _atk) => atk = _atk);

            warrior.ATK += atk;
            if (warrior.ATK < 0)
                warrior.ATK = 0;

            OnAddATK?.Invoke(warrior, atk);
            AfterAddATK?.Invoke(warrior, atk);
        }

        #endregion
    }
}