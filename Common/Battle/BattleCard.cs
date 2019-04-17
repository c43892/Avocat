using System;
using System.Collections;
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
        public static readonly BattleCard Energy = new EN();
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
                    return new EN();
            }

            return null;
        }

        public static BattleCard Create(string type)
        {
            switch (type)
            {
                case "HP":
                    return new BattleCardHP();
                case "ES":
                    return new BattleCardES();
                case "ATK":
                    return new BattleCardATK();
                case "EN":
                    return new EN();
            }

            return null;
        }

        public string Name { get; protected set; }

        public abstract IEnumerator ExecuteOn(Warrior warrior);
    }

    public class BattleCardHP : BattleCard
    {
        public BattleCardHP() { Name = "HP"; }
        public override IEnumerator ExecuteOn(Warrior warrior)
        {
            yield return warrior.Battle.AddHP(warrior, 1);
        }
    }

    public class BattleCardATK : BattleCard
    {
        public BattleCardATK() { Name = "ATK"; }
        public override IEnumerator ExecuteOn(Warrior warrior)
        {
            var bt = warrior.Battle;
            var dATK = (int)(warrior.ATK * 0.15);
            dATK = dATK == 0 ? 1 : dATK;
            yield return bt.AddBuff(new CardATK() { ATK = dATK }, warrior);
        }
    }

    public class BattleCardES : BattleCard
    {
        public BattleCardES() { Name = "ES"; }
        public override IEnumerator ExecuteOn(Warrior warrior)
        {
            yield return warrior.Battle.AddES(warrior, warrior.MaxES / 4);
        }
    }

    public class EN : BattleCard
    {
        public EN() { Name = "EN"; }
        public override IEnumerator ExecuteOn(Warrior warrior)
        {
            yield return (warrior.Battle as BattlePVE).AddEN(15);
        }
    }
}
