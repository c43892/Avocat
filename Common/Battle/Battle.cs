﻿using System;
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

            foreach (var buff in warrior.Buffs)
                buff.OnDetached();

            Map.RemoveObj(warrior);

            OnWarriorRemoved?.Invoke(warrior);
            AfterWarriorRemoved?.Invoke(warrior);
        }

        // 生成随机战斗卡牌
        public BattleCard[] GenNextCards(int num)
        {
            var cards = new BattleCard[num];
            FC.For(num, (i) =>
            {
                var type = Srand.Next(0, BattleCard.BattleCardTypesNum);
                cards[i] = BattleCard.Create(BattleCard.CardTypes[type]);
            });

            return cards;
        }

        #region 战斗准备过程

        // 移动角色位置
        public void MoveWarroirs(Warrior warrior, int tx, int ty)
        {
            warrior.GetPosInMap(out int fx, out int fy);
            if (fx == tx && fy == ty)
                return;

            Debug.Assert(!map.BlockedAt(tx, ty, warrior.StandableTiles), "target position has been blocked: " + tx + ", " + ty);

            Map.SetObjAt(fx, fy, null);
            Map.SetObjAt(tx, ty, warrior);
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

            var tmp = Map.GetAt<Warrior>(fx, fy);
            Map.SetObjAt(fx, fy, Map.GetAt<Warrior>(tx, ty));
            Map.SetObjAt(tx, ty, tmp);

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
                var warrior = Map.GetAt<Warrior>(x, y);
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
        public event Action<Warrior, Action<bool, bool, bool>> BeforeResetActionFlag = null;
        public event Action<Warrior> AfterResetActionFlag = null;
        public event Action<Warrior> DoResetActionFlag = null;
        public void ResetActionFlag(Warrior warrior)
        {
            var resetMovedFlag = true; // 移动标记
            var resetActionFlag = true; // 行动标记
            var resetSkillFlag = true; // 技能释放标记

            BeforeResetActionFlag?.Invoke(warrior, (_resetMovedFlag, _resetActionFlag, _resetSkillFlag) =>
            {
                resetMovedFlag = _resetMovedFlag;
                resetActionFlag = _resetActionFlag;
                resetSkillFlag = _resetSkillFlag;
            });

            SetActionFlag(warrior, 
                resetMovedFlag ? false : warrior.Moved,
                resetActionFlag ? false : warrior.ActionDone,
                resetSkillFlag ? false : warrior.IsSkillReleased);

            DoResetActionFlag?.Invoke(warrior);
            AfterResetActionFlag?.Invoke(warrior);
        }

        // 回合开始，重置所有角色行动标记
        public event Action<int> BeforeStartNextRound1 = null;
        public event Action<int> BeforeStartNextRound2 = null;
        public event Action<int> AfterStartNextRound = null;
        public event Action<int> OnNextRoundStarted = null;
        public void StartNextRound(int player)
        {
            BeforeStartNextRound1?.Invoke(player);
            BeforeStartNextRound2?.Invoke(player);

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
        public event Action<Warrior, bool> BeforeMoveOnPath = null;
        public event Action<Warrior, int, int, List<int>, bool> AfterMoveOnPath = null;
        public event Action<Warrior, int, int, List<int>, bool> OnWarriorMovingOnPath = null; // 角色沿路径移动
        public List<int> MoveOnPath(Warrior warrior, bool forceMove = false /* 忽略限制强制移动 */)
        {
            Debug.Assert(forceMove || (!warrior.Moved && !warrior.ActionDone), "attacker has already moved or acted in this round");

            warrior.GetPosInMap(out int x, out int y);
            Debug.Assert(MU.ManhattanDist(x, y, warrior.MovingPath[0], warrior.MovingPath[1]) == 1, "the warrior has not been right on the start position: " + x + ", " + y);

            if (!forceMove && warrior.MovingPath.Count > warrior.MoveRange * 2) // 超出移动能力
                return null;

            BeforeMoveOnPath?.Invoke(warrior, forceMove);

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

                Debug.Assert(!map.BlockedAt(tx, ty, warrior.StandableTiles), "target position has been blocked: " + tx + ", " + ty);

                MoveWarroirs(warrior, tx, ty);

                fx = tx;
                fy = ty;
            }

            warrior.Moved = true;

            OnWarriorMovingOnPath?.Invoke(warrior, x, y, movedPath, forceMove);
            AfterMoveOnPath?.Invoke(warrior, x, y, movedPath, forceMove);
            return movedPath;
        }

        // 计算伤害
        public delegate void CalculateDamageAction(Warrior attacker, Warrior target, HashSet<string> flags, ref int inc, ref int more, List<int> muliplier);
        public event CalculateDamageAction BeforeCalculateDamage1;
        public event CalculateDamageAction BeforeCalculateDamage2;
        public int CalculateDamage(Warrior attacker, Warrior target, Skill skill, HashSet<string> flags, List<int> multiplier /* 叠加系数，包括减伤系数，反击伤害系数，溅射系数等 */)
        {
            // 物理和法术分别取不同的抗性，混乱攻击忽视抗性

            var basicAttack = attacker.BasicAttackValue;
            if (skill is ISkillWithAXY)
                basicAttack = Calculation.CalcBasicAttackByAXY(attacker, skill as ISkillWithAXY);

            var inc = 0;
            var more = 0;

            if (flags.Contains("physic"))
            {
                inc = attacker.ATKInc;
                more = attacker.ATKMore;
                multiplier.Add(target.ARM);
            }
            else if (flags.Contains("magic"))
            {
                inc = attacker.POWInc;
                more = attacker.POWMore;
                multiplier.Add(target.RES);
            }

            if (flags.Contains("CriticalAttack"))  // 暴击系数
                multiplier.Add(attacker.Crit);

            // 通知所有可能影响各种系数的计算逻辑
            BeforeCalculateDamage1?.Invoke(attacker, target, flags, ref inc, ref more, multiplier);
            BeforeCalculateDamage2?.Invoke(attacker, target, flags, ref inc, ref more, multiplier);

            // 计算最终攻击值
            return Calculation.CalcDamage(basicAttack, inc, more, multiplier);
        }

        // 变更行动标记
        public event Action<Warrior, bool, bool, bool> BeforeSetActionFlag = null;
        public event Action<Warrior, bool, bool, bool> AfterSetActionFlag = null;
        public event Action<Warrior, bool, bool, bool> OnSetActionFlag = null;
        public void SetActionFlag(Warrior warrior, bool moved, bool acted, bool skillReleased)
        {
            BeforeSetActionFlag?.Invoke(warrior, moved, acted, skillReleased);
            warrior.Moved = moved;
            warrior.ActionDone = acted;
            warrior.IsSkillReleased = skillReleased;
            OnSetActionFlag?.Invoke(warrior, moved, acted, skillReleased);
            AfterSetActionFlag?.Invoke(warrior, moved, acted, skillReleased);
        }

        // 角色沿路径移动后执行攻击动作
        public event Action<Warrior, Warrior> BeforeMoveOnPathAndAttack = null;
        public event Action<Warrior, Warrior, int, int, List<int>> AfterMoveOnPathAndAttack = null;
        public event Action<Warrior, Warrior, int, int, List<int>> OnMoveOnPathAndAttack = null;
        public void MoveOnPathAndAttack(Warrior attacker, Warrior target, Skill skill = null, params string[] flags) { MoveOnPathAndAttack(attacker, target, null, skill, flags); }
        public void MoveOnPathAndAttack(Warrior attacker, Warrior target, Warrior[] addtionalTargets, Skill skill = null, params string[] flags)
        {
            attacker.GetPosInMap(out int fx, out int fy);
            BeforeMoveOnPathAndAttack?.Invoke(attacker, target);

            var pathList = MoveOnPath(attacker);
            Attack(attacker, target, addtionalTargets, skill, flags);

            OnMoveOnPathAndAttack?.Invoke(attacker, target, fx, fy, pathList);
            AfterMoveOnPathAndAttack?.Invoke(attacker, target, fx, fy, pathList);
        }

        // 执行攻击
        public event Action<Warrior, Warrior, List<Warrior>, Skill, HashSet<string>> PrepareAttack = null;
        public event Action<Warrior, Warrior, List<Warrior>, Skill, HashSet<string>, List<int>, List<int>> BeforeAttack = null;
        public event Action<Warrior, Warrior, List<Warrior>, Skill, HashSet<string>> AfterAttack = null;
        public event Action<Warrior, Warrior, List<Warrior>, Dictionary<Warrior, int>, Dictionary<Warrior, int>, Skill, HashSet<string>> OnWarriorAttack = null; // 角色进行攻击
        public void Attack(Warrior attacker, Warrior target, Skill skill = null, params string[] flags) { Attack(attacker, target, null, skill, flags); }
        public void Attack(Warrior attacker, Warrior target, Warrior[] addtionalTargets, Skill skill = null, params string[] flags)
        {
            // 在连续指令的情况下，可能条件已经不满足
            if (attacker == null || target == null)
                return;

            // 整理攻击标记
            var attackFlags = new HashSet<string>();
            attackFlags.UnionWith(flags);
            attackFlags.Add(attacker.AttackingType);

            // 额外攻击目标
            var addTars = new List<Warrior>();
            if (addtionalTargets != null)
                addTars.AddRange(addtionalTargets);

            PrepareAttack?.Invoke(attacker, target, addTars, skill, attackFlags);

            if (!attackFlags.Contains("IgnoreAttackRange")) // 可能是无视攻击距离限制的
            {
                target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
                if (!attacker.InAttackRange(tx, ty))
                    return;
            }

            // 额外伤害系数
            var multi = new List<int>();
            var addMulti = new List<int>();
            BeforeAttack?.Invoke(attacker, target, addTars, skill, attackFlags, multi, addMulti);

            // 可能需要取消攻击 或者因为 PatternSkill 导致已经行动过了
            if (attacker.ActionDone || attackFlags.Contains("CancelAttack"))
                return;

            var tars = new List<Warrior>();
            tars.Add(target);
            tars.AddRange(addTars);

            // 计算实际伤害
            var damages = new Dictionary<Warrior, int>();
            foreach (var tar in tars)
                damages[tar] = CalculateDamage(attacker, tar, skill, attackFlags, tar == target ? multi : addMulti);

            // ExtraAttack 不影响行动标记
            if (!attackFlags.Contains("ExtraAttack"))
            {
                Debug.Assert(!attacker.ActionDone, "attacker has already attacted in this round");
                SetActionFlag(attacker, true, true, true);
            }

            // 混乱攻击不计算护盾，其它类型攻击需要先消耗护盾
            var dhps = new Dictionary<Warrior, int>();
            var dess = new Dictionary<Warrior, int>();
            foreach (var tar in tars)
            {
                var damage = damages[tar];
                var dhp = -damage;
                var des = 0;
                if (!attackFlags.Contains("chaos") && target.ES > 0)
                {
                    dhp = damage > target.ES ? target.ES - damage : 0;
                    des = damage > target.ES ? 0 : -damage;
                }

                dhps[tar] = dhp;
                dess[tar] = des;
            }

            OnWarriorAttack?.Invoke(attacker, target, addTars, dhps, dess, skill, attackFlags);

            foreach (var tar in tars)
            {
                var dhp = dhps[tar];
                var des = dess[tar];

                if (des != 0) AddES(tar, des);
                if (dhp != 0) AddHP(tar, dhp);
            }

            AfterAttack?.Invoke(attacker, target, addTars, skill, attackFlags);

            return;
        }

        // 模拟攻击行为的伤害数值，但并不执行攻击行为
        public void SimulateAttackingDamage(Warrior attacker, Warrior target, Warrior[] addtionalTargets, Skill skill, Action<int> onDamage, params string[] flags)
        {
            target.GetPosInMap(out int tx, out int ty); // 检查攻击范围限制
            if (!attacker.InAttackRange(tx, ty))
                return;

            // 整理攻击标记
            var attackFlags = new HashSet<string>();
            attackFlags.UnionWith(flags);
            attackFlags.Add(attacker.AttackingType);

            var addTars = new List<Warrior>();
            if (addtionalTargets != null)
                addTars.AddRange(addtionalTargets);

            // 额外伤害系数
            var multi = new List<int>();
            var addMulti = new List<int>();
            BeforeAttack?.Invoke(attacker, target, addTars, skill, attackFlags, multi, addMulti);

            if (attackFlags.Contains("CancelAttack")) // 取消攻击
                return;

            // 计算实际伤害
            var damage = CalculateDamage(attacker, target, skill, attackFlags, multi);
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

            Map.SetObjAt(x, y, warrior);

            if (warrior.AI != null)
                ResetWarriorAI(warrior);

            OnAddWarrior?.Invoke(x, y, warrior);
            AfterAddWarrior?.Invoke(x, y, warrior);
        }

        // 添加道具到地图
        public event Action<int, int, BattleMapObj> BeforeAddItem = null;
        public event Action<int, int, BattleMapObj> AfterAddItem = null;
        public event Action<int, int, BattleMapObj> OnAddItem = null;
        public void AddItemAt(int x, int y, BattleMapObj item)
        {
            BeforeAddItem?.Invoke(x, y, item);

            Map.SetObjAt(x, y, item);

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
        public virtual Buff AddBuff(Buff buff)
        {
            var target = buff is ISkillWithOwner ? (buff as ISkillWithOwner).Owner : null;

            BeforeBuffAttached?.Invoke(buff, target);

            if (target != null)
            {
                target.AddOrOverBuffInternal(ref buff); // 这个时候可能发生同名 buff 回合数叠加，buff 对象会变为已经在目标身上的原 buff
                Debug.Assert(buff != null, "a buff was replaced with a null");
            }
            else
            {
                Debug.Assert(!GroundBuffs.Contains(buff), "buff " + buff.ID + " already attached to ground");
                GroundBuffs.Add(buff);
            }

            buff.OnAttached();

            OnBuffAttached?.Invoke(buff, target);
            AfterBuffAttached?.Invoke(buff, target);

            return buff;
        }

        /// <summary>
        /// 移除 buff 或被动技能效果
        /// </summary>
        public virtual void RemoveBuffByID(Warrior target, string ID)
        {
            if (target.GetBuffByID(ID) is Buff buff)
                RemoveBuff(buff);
        }

        protected event Action<Buff, Warrior> BeforeBuffRemoved = null;
        protected event Action<Buff, Warrior> AfterBuffRemoved = null;
        public event Action<Buff, Warrior> OnBuffRemoved = null;
        public virtual void RemoveBuff(Buff buff)
        {
            var target = buff is ISkillWithOwner ? (buff as ISkillWithOwner).Owner : null;

            BeforeBuffRemoved?.Invoke(buff, target);

            if (target != null)
            {
                Debug.Assert(target.Buffs.Contains(buff), "buff " + buff.ID + " has not been attached to target (" + target.AvatarID + "," + target.IDInMap + ")");
                target.RemoveBuffInternal(buff);
            }
            else
            {
                Debug.Assert(GroundBuffs.Contains(buff), "buff " + buff.ID + " has not been attached to ground)");
                GroundBuffs.Remove(buff);
            }

            buff.OnDetached();

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
            AddBuff(new ResetES(this)); // 回合开始时重置护盾
            AddBuff(new ResetActionFlag(this)); // 回合开始时重置行动标记
            return this;
        }

        #endregion

        #region 角色操作包装

        // 角色加血
        public event Action<Warrior, int, Action<int>> BeforeAddHP = null;
        public event Action<Warrior, int> AfterAddHP = null;
        public event Action<Warrior, int> OnAddHP = null;
        public event Action<Warrior> BeforeWarriorDying = null; // 角色死亡
        public event Action<Warrior> OnWarriorDying = null; // 角色死亡
        public event Action<Warrior> AfterWarriorDead = null; // 角色死亡
        public void AddHP(Warrior warrior, int dhp)
        {
            BeforeAddHP?.Invoke(warrior, dhp, (int _dhp) => dhp = _dhp);

            warrior.HP = (warrior.HP + dhp).Clamp(0, warrior.MaxHP);

            OnAddHP?.Invoke(warrior, dhp);
            AfterAddHP?.Invoke(warrior, dhp);

            if (warrior.IsDead)
            {
                BeforeWarriorDying?.Invoke(warrior);
                OnWarriorDying?.Invoke(warrior);
                AfterWarriorDead?.Invoke(warrior);
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