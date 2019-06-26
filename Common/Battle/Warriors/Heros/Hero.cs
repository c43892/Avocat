﻿using Swift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 所有英雄角色
    /// </summary>
    public abstract class Hero : Warrior
    {
        public Hero(Battle bt)
            : base(bt.Map)
        {
            StandableTiles = TileType.All;
        }

        public void AddSkill(params Skill[] ss)
        {
            foreach (var s in ss)
            {
                if (s is ActiveSkill)
                    AddActiveSkill(Configuration.Config(s as ActiveSkill));
                else if (s is Buff)
                    Battle.AddBuff(Configuration.Config(s as Buff), this);
            }
        }

        // 刷什么类型的卡
        public virtual string CardType { get; set; }

        #region 符文相关

        // 进入战斗前，执行所有符文效果
        public void RunAllRune2PrepareBattle()
        {
            foreach (var rune in runes)
                rune.OnPreparingBattle(this);
        }

        List<Rune> runes = new List<Rune>();

        public void AddRune(Rune rune)
        {
            runes.Add(rune);
        }

        public void RemoveRune(Rune rune)
        {
            runes.Remove(rune);
        }

        public Rune[] AllRunes
        {
            get
            {
                return runes.ToArray();
            }
        }

        #endregion
    }
}
