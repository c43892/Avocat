﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 巴洛克
    /// 战术指挥
    /// </summary>
    public class BaLuoKe : Hero, ITransformable
    {
        public BaLuoKe(Battle bt)
            : base(bt)
        {
            ID = "BaLuoKe";
            SetupSkills(new FastAssistance1(this), 
                new TacticalCommand(this) {
                    Impl = new TacticalCommandImpl1(() => 
                        State == "Archer" ? new string[] { "EN" } : new string[] { "ATK" })
                }, new CounterAttack(this));
        }
        
        public int[] ArcherAttackRange { get; set; }
        public int ArcherATK { get; set; }
        public int[] LancerAttackRange { get; set; }
        public int LancerATK { get; set; }

        public string State {
            get
            {
                return state;
            }
            set
            {
                state = value;
                switch (value)
                {
                    case "Archer": // 弓手
                        AttackRange = ArcherAttackRange;
                        ATK = ArcherATK;
                        break;
                    case "Lancer": // 枪兵
                        AttackRange = LancerAttackRange;
                        ATK = LancerATK;
                        break;
                }
            }
        } string state;
    }
}
