using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 战斗中的卡牌
    /// </summary>
    public abstract class BattleCard
    {
        public static readonly BattleCard Power = new BattleCardPower();
        public static readonly BattleCard Defence = new BattleCardDefence();
        public static readonly BattleCard Hp = new BattleCardHp();
        public static readonly BattleCard Energy = new BattleCardEnergy();
        public static readonly int BattleCardTypesNum = 4;

        public static BattleCard Create(int type)
        {
            switch (type)
            {
                case 0:
                    return new BattleCardHp();
                case 1:
                    return new BattleCardDefence();
                case 2:
                    return new BattleCardPower();
                case 3:
                    return new BattleCardEnergy();
            }

            return null;
        }

        public string Name { get; protected set; }

        public abstract void ExecuteOn(Warrior warrior);
    }

    public class BattleCardHp : BattleCard
    {
        public BattleCardHp() { Name = "hp"; }
        public override void ExecuteOn(Warrior warrior)
        {
            if (warrior.Hp < warrior.MaxHp)
                warrior.Hp += 1;
        }
    }

    public class BattleCardPower : BattleCard
    {
        public BattleCardPower() { Name = "power"; }
        public override void ExecuteOn(Warrior warrior)
        {
            warrior.Power += 1;
        }
    }

    public class BattleCardDefence : BattleCard
    {
        public BattleCardDefence() { Name = "defence"; }
        public override void ExecuteOn(Warrior warrior)
        {
            warrior.Shield += 1;
        }
    }

    public class BattleCardEnergy : BattleCard
    {
        public BattleCardEnergy() { Name = "energy"; }
        public override void ExecuteOn(Warrior warrior)
        {
        }
    }
}
