using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 普通 PVE 战斗
    /// </summary>
    public class BattlePVE : Battle
    {
        public PlayerInfo Player { get; protected set; }
        protected RobotPlayer Robot { get; set; }

        public static readonly int PlayerIndex = 1; // 玩家是 1，机器人是 2

        // 玩家当前可用战斗卡牌
        public List<BattleCard> AvailableCards = new List<BattleCard>();

        // 暂存区的卡牌
        public List<BattleCard> StashedCards = new List<BattleCard>();

        // 玩家最大技能卡数量
        public int PlayerMaxCardNum { get; } = 8;

        // 玩家能量槽
        public int MaxEnergy { get; set; } = 100;
        public int Energy { get; set; } = 0;

        // 玩家建设值，由卡牌分解生成，可用于使用地图上的道具
        public int MaxCardUsage { get; set; } = 100;
        public int CardUsage { get; set; } = 0;

        public BattlePVE(BattleMap map, int randSeed, PlayerInfo player)
            :base(map, randSeed)
        {
            Player = player;
            Build();
        }

        bool playerPrepared = false;
        public override void PlayerPreparedImpl(int player)
        {
            Debug.Assert(!playerPrepared, "player already prepared in PVE");
            playerPrepared = true;
        }

        public override bool AllPrepared
        {
            get
            {
                return playerPrepared;
            }
        }

        // 添加卡片
        public void AddCards(IEnumerable<BattleCard> cards = null)
        {
            if (cards != null)
                AvailableCards.AddRange(cards);

            // 限制最大数量
            if (AvailableCards.Count > PlayerMaxCardNum)
            {
                var removeNum = AvailableCards.Count - PlayerMaxCardNum;
                AvailableCards.RemoveRange(AvailableCards.Count - removeNum, removeNum);
            }

            var moveRange = AvailableCards.Count;
            Map.ForeachObjs<Warrior>((x, y, warrior) =>
            {
                if (warrior.Team == PlayerIndex)
                    warrior.MoveRange = moveRange;
            });
        }

        // 创建 PVE 战斗逻辑
        protected void Build()
        {
            Build(1, 2);  // 1-玩家，2-机器人

            ConsumeCardsOnMoving();

            // 行动结束分解剩余卡牌
            AddBuff(new DisassembleCards(this, AvailableCards));

            // 行动开始前，生成新卡牌
            AddBuff(new GenCards(this));

            Robot = new RobotPlayer(this);
        }

        // 移动消耗卡牌
        public event Action<Warrior, List<BattleCard>> BeforeCardsConsumed = null;
        public event Action<Warrior, List<BattleCard>> AfterCardsConsumed = null;
        public event Action<Warrior, List<BattleCard>> OnCardsConsumed = null;
        void OnAfterMoveOnPath(Warrior warrior, int fx, int fy, List<int> movedPath, bool ignoreMoveRangeRestrict)
        {
            if (warrior.Team != PlayerIndex || ignoreMoveRangeRestrict)
                return;

            var movedPathLen = movedPath.Count / 2;
            Debug.Assert(movedPathLen <= AvailableCards.Count, "moved path grids should not be more than cards number");

            var cards = new List<BattleCard>();
            for (var i = 0; i < movedPathLen; i++)
            {
                var card = AvailableCards[0];
                cards.Add(card);
                AvailableCards.RemoveAt(0);
            }

            BeforeCardsConsumed?.Invoke(warrior, cards);

            for (var i = 0; i < cards.Count; i++)
                cards[i].ExecuteOn(warrior);

            OnCardsConsumed?.Invoke(warrior, cards);

            AddCards();

            AfterCardsConsumed?.Invoke(warrior, cards);
        }

        BattlePVE ConsumeCardsOnMoving()
        {
            AfterMoveOnPath += OnAfterMoveOnPath;
            return this;
        }

        // 交换战斗卡牌位置
        public event Action<int, int, int, int> BeforeBattleCardsExchange = null;
        public event Action<int, int, int, int> AfterBattleCardsExchange = null;
        public event Action<int, int, int, int> OnBattleCardsExchange = null;
        public void ExchangeBattleCards(int g1, int n1, int g2, int n2)
        {
            BeforeBattleCardsExchange?.Invoke(g1, n1, g2, n2);

            var lst1 = g1 == 0 ? AvailableCards : StashedCards;
            var lst2 = g2 == 0 ? AvailableCards : StashedCards;
            var c1 = n1 < lst1.Count ? lst1[n1] : null;
            var c2 = n2 < lst2.Count ? lst2[n2] : null;

            if (n1 < lst1.Count)
                lst1[n1] = c2;
            else
                lst1.Add(c2);

            if (n2 < lst2.Count)
                lst2[n2] = c1;
            else
                lst2.Add(c1);

            for (var i = lst1.Count - 1; i >= 0; i--)
                if (lst1[i] == null)
                    lst1.RemoveAt(i);

            for (var i = lst2.Count - 1; i >= 0; i--)
                if (lst2[i] == null)
                    lst2.RemoveAt(i);

            n1 = lst1.IndexOf(c2);
            n2 = lst2.IndexOf(c1);

            AddCards();

            OnBattleCardsExchange?.Invoke(g1, n1, g2, n2);
            AfterBattleCardsExchange?.Invoke(g1, n1, g2, n2);
        }

        // 增加一张战斗卡牌
        public event Action<BattleCard> BeforeAddBattleCard = null;
        public event Action<BattleCard> AfterAddBattleCard = null;
        public event Action<BattleCard> OnAddBattleCard = null;
        public void AddBattleCard(BattleCard card)
        {
            BeforeAddBattleCard?.Invoke(card);

            AvailableCards.Add(card);
            AddCards();

            OnAddBattleCard?.Invoke(card);
            AfterAddBattleCard?.Invoke(card);
        }

        // 根据英雄技能组合自动排序卡牌
        public event Action<Warrior , List<BattleCard>> BeforeSortBattleCard = null;
        public event Action<Warrior, List<BattleCard>> AfterSortBattleCard = null;
        public event Action<Warrior, List<BattleCard>> OnSortBattleCard = null;
        public void SortBattleCard(Warrior warrior)
        {
            BeforeSortBattleCard?.Invoke(warrior,AvailableCards);

            AutoSortCards(warrior);

            OnSortBattleCard?.Invoke(warrior, AvailableCards);
            AfterSortBattleCard?.Invoke(warrior, AvailableCards);
        }

        public void AutoSortCards(Warrior warrior)
        {
            // 没有pattern技能则不自动排序
            if (warrior.PatternSkill == null)
                return;

            BattleCard [] AfterSortingCards = new BattleCard[AvailableCards.Count];
            var skillOrder = warrior.PatternSkill.CardsPattern;
            bool[] visited = new bool[AvailableCards.Count];
            int currentIndex =0;
            int countNumber = 0;
            int[] StoreIndex = new int[AvailableCards.Count];

            while (countNumber < AvailableCards.Count)
            {
                for (int i = 0; i < skillOrder.Length; i++)
                {
                    for (int j = 0; j < AvailableCards.Count; j++)
                    {
                        if (!visited[j] && AvailableCards[j].Name.Equals(skillOrder[i]))
                        {
                            visited[j] = true;
                            StoreIndex[currentIndex] = j; // 记录下排序后当前位置的卡牌原来的index
                            currentIndex++;
                            break;
                        }
                    }
                    countNumber++;
                }

                // 判断是否应该将StoreIndex储存的卡牌更新到排序后的卡组
                if (currentIndex % skillOrder.Length == 0 && currentIndex != 0)
                {
                    FC.For(currentIndex - skillOrder.Length, currentIndex, (i) =>
                    {
                        var j = StoreIndex[i];
                        AfterSortingCards[i] = AvailableCards[j];
                    });
                }
                else // 将不符合组合的卡牌退回到剩余卡牌中
                {
                    var remainder = currentIndex % skillOrder.Length;

                    FC.For(currentIndex - remainder, currentIndex, (i) =>
                    {
                        visited[StoreIndex[i]] = false;
                    });
                    currentIndex = currentIndex - remainder;
                    break;
                }                   
            }

            // 将不符合组合条件的剩余卡牌放入排序后的卡组
            for (int i = currentIndex; i < AfterSortingCards.Length; i++)
            {
                for (int j = 0; j < AvailableCards.Count; j++)
                {
                    if (!visited[j])
                    {
                        visited[j] = true;
                        AfterSortingCards[i] = AvailableCards[j];
                        break;
                    }
                }
            }

            // 将排序后的卡组赋给原来卡组
            FC.For(AvailableCards.Count, (i) =>
            {
                AvailableCards[i] = AfterSortingCards[i];
            });
        }

        #region 角色操作包装

        // 玩家加能量
        public event Action<int, Action<int>> BeforeAddEN = null;
        public event Action<int> AfterAddEN = null;
        public event Action<int> OnAddEN = null;
        public void AddEN(int den)
        {
            BeforeAddEN?.Invoke(den, (int _den) => den = _den);
            Energy = (Energy + den).Clamp(0, MaxEnergy);
            OnAddEN?.Invoke(den);
            AfterAddEN?.Invoke(den);
        }

        // 玩家加建设值
        public event Action<int, Action<int>> BeforeAddCardDissambleValue = null;
        public event Action<int> AfterAddCardDissambleValue = null;
        public event Action<int> OnAddCardDissambleValue = null;
        public void AddCardDissambleValue(int dv)
        {
            BeforeAddCardDissambleValue?.Invoke(dv, (int _dv) => dv = _dv);

            CardUsage = (CardUsage + dv).Clamp(0, MaxCardUsage);

            OnAddCardDissambleValue?.Invoke(dv);
            AfterAddCardDissambleValue?.Invoke(dv);
        }

        // 释放主动技能
        public event Action<ActiveSkill> BeforeFireSkill = null;
        public event Action<ActiveSkill> AfterFireSkill = null;
        public event Action<ActiveSkill> OnFireSkill = null;
        public virtual void FireSkill(ActiveSkill skill)
        {
            if (Energy < skill.EnergyCost)
                return;

            if (skill.Owner.IsSkillReleased)
                return;

            BeforeFireSkill?.Invoke(skill);

            AddEN(-skill.EnergyCost);
            skill.Owner.IsSkillReleased = true;
            skill.Fire();

            OnFireSkill?.Invoke(skill);
            AfterFireSkill?.Invoke(skill);
        }

        // 释放主动技能
        public event Action<ActiveSkill, int, int> BeforeFireSkillAt = null;
        public event Action<ActiveSkill, int, int> AfterFireSkillAt = null;
        public event Action<ActiveSkill, int, int> OnFireSkillAt = null;
        public virtual void FireSkillAt(ActiveSkill skill, int x, int y)
        {
            if (Energy < skill.EnergyCost)
                return;

            if (skill.Owner.IsSkillReleased)
                return;

            BeforeFireSkillAt?.Invoke(skill, x, y);

            AddEN(-skill.EnergyCost);
            (skill as ISkillWithPosSel).FireAt(x, y);
            skill.Owner.IsSkillReleased = true;

            OnFireSkillAt?.Invoke(skill, x, y);
            AfterFireSkillAt?.Invoke(skill, x, y);
        }

        // 使用道具
        public event Action<ItemOnMap, Warrior> BeforeUseItem2 = null;
        public event Action<ItemOnMap, Warrior> AfterUseItem2 = null;
        public event Action<ItemOnMap, Warrior> OnUseItem2 = null;
        public virtual void UseItem2(ItemOnMap item, Warrior target)
        {
            if (CardUsage < MaxCardUsage)
                return;

            BeforeUseItem2?.Invoke(item, target);

            AddCardDissambleValue(-CardUsage);
            item.Use2(target);

            OnUseItem2?.Invoke(item, target);
            AfterUseItem2?.Invoke(item, target);
        }

        #endregion
    }
}
