﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 眩晕
    /// 没行动能力
    /// </summary>
    public class Faint : CountDownBuffWithOwner
    {
        public override string ID { get; } = "Faint";
        public Faint(Warrior owner, int num) : base(owner, num) { }
        
        void UnsetActionFlag(Warrior warrior, Action<bool, bool, bool> resetActionFlags)
        {
            if (warrior != Owner)
                return;

            resetActionFlags(false, false, false);
        }

        void CancelAttack(Warrior attacker, Warrior target, List<Warrior> tars, Skill skill, HashSet<string> flags, List<int> multi, List<int> addMulti)
        {
            if (attacker != Owner)
                return;

            flags.Add("CancelAttack");
        }

        public override void OnAttached()
        {
            Battle.BeforeResetActionFlag += UnsetActionFlag;
            Battle.BeforeAttack += CancelAttack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeResetActionFlag -= UnsetActionFlag;
            Battle.BeforeAttack -= CancelAttack;
            base.OnDetached();
        }
    }
}
