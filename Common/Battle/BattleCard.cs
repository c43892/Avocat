using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// 战斗中的卡牌
    /// </summary>
    public abstract class BattleCard
    {
        public static readonly string[] CardTypes = new string[] { "ATK", "ES", "PT", "EN" };
        public static int BattleCardTypesNum { get => CardTypes.Length; }

        public static BattleCard Create(string type)
        {
            switch (type)
            {
                case "PT":
                    return new BattleCardPotion();
                case "ES":
                    return new BattleCardES();
                case "ATK":
                    return new BattleCardATK();
                case "EN":
                    return new BattleCardEN();
            }

            throw new Exception("unknown card type: " + type);
        }

        public string Name { get; protected set; }
        public bool visited = false;
        public abstract void ExecuteOn(Warrior warrior);
    }

    public class BattleCardATK : BattleCard
    {
        public BattleCardATK() { Name = "ATK"; }
        public override void ExecuteOn(Warrior warrior)
        {
            var bt = warrior.Battle;
            var dATK = (warrior.ATK * 15 / 100).Clamp(1, int.MaxValue);
            bt.AddBuff(new CardATK() { ATK = dATK }, warrior);
        }
    }

    public class BattleCardES : BattleCard
    {
        public BattleCardES() { Name = "ES"; }
        public override void ExecuteOn(Warrior warrior)
        {
            var dES = (warrior.MaxES * 25 / 100).Clamp(1, int.MaxValue);
            warrior.Battle.AddES(warrior, dES);
        }
    }

    public class BattleCardPotion : BattleCard
    {
        public BattleCardPotion() { Name = "PT"; }
        public override void ExecuteOn(Warrior warrior)
        {
            warrior.Battle.AddHP(warrior, 15);
        }
    }

    public class BattleCardEN : BattleCard
    {
        public BattleCardEN() { Name = "EN"; }
        public override void ExecuteOn(Warrior warrior)
        {
            (warrior.Battle as BattlePVE).AddEN(15);
        }
    }
}
