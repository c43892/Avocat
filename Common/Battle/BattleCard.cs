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
    public class BattleCard
    {
        public static readonly BattleCard Power = new BattleCard("+P");
        public static readonly BattleCard Defence = new BattleCard("+D");
        public static readonly BattleCard Hp = new BattleCard("+Hp");
        public static readonly BattleCard Energy = new BattleCard("+E");

        public static readonly BattleCard[] AllTypeOfBattleCards = new BattleCard[] { BattleCard.Power, BattleCard.Defence, BattleCard.Hp, BattleCard.Energy };

        public string Name { get; private set; }
        private BattleCard(string name)
        {
            Name = name;
        }
    }
}
