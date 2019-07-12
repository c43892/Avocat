using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 玩家角色
    /// </summary>
    public class Player
    {
        #region 装备相关

        // 已装备的装备
        readonly List<Equip> Equipped = new List<Equip>();
        public void AddEquip(Equip equip)
        {
            equip.Team = 1;
            Equipped.Add(equip);
        }

        #endregion

        // 战斗准备
        public void OnPreparingBattle(Battle bt)
        {
            foreach (var equip in Equipped)
                equip.OnPreparingBattle(bt);
        }
    }
}
