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
        public static readonly BattleCard ATK = new BattleCardATK();
        public static readonly BattleCard ES = new BattleCardES();
        public static readonly BattleCard HP = new BattleCardHP();
        public static readonly BattleCard Energy = new BattleCardPOW();
        public static readonly int BattleCardTypesNum = 4;

        public static BattleCard Create(int type)
        {
            switch (type)
            {
                case 0:
                    return new BattleCardHP();
                case 1:
                    return new BattleCardES();
                case 2:
                    return new BattleCardATK();
                case 3:
                    return new BattleCardPOW();
            }

            return null;
        }

        public string Name { get; protected set; }

        public abstract void ExecuteOn(Warrior warrior);
    }

    public class BattleCardHP : BattleCard
    {
        public BattleCardHP() { Name = "HP"; }
        public override void ExecuteOn(Warrior warrior)
        {
            if (warrior.Hp < warrior.MaxHp)
                warrior.Hp += 1;
        }
    }

    public class BattleCardATK : BattleCard
    {
        public BattleCardATK() { Name = "ATK"; }
        public override void ExecuteOn(Warrior warrior)
        {
            warrior.ATK += 1;
        }
    }

    public class BattleCardES : BattleCard
    {
        public BattleCardES() { Name = "ES"; }
        public override void ExecuteOn(Warrior warrior)
        {
            warrior.ES += 1;
        }
    }

    public class BattleCardPOW : BattleCard
    {
        public BattleCardPOW() { Name = "POW"; }
        public override void ExecuteOn(Warrior warrior)
        {
        }
    }
}
