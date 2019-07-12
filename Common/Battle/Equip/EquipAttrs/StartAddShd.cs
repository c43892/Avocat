using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 回合开始前，获得 n% 最大护盾值
    /// </summary>
    public class StartAddShd : EquipAttr
    {
        public int P0 { get; set; } // 护盾值百分比

        public override void OnPreparingBattle(Battle bt)
        {
            bt.BeforeStartNextRound1 += (int team) =>
            {
                if (team != Team)
                    return;

                bt.Map.ForeachObjs<Warrior>((x, y, warrior) =>
                {
                    bt.AddES(warrior, warrior.MaxES * P0 / 100);
                }, (warrior) => warrior.Team == team);
            };
        }
    }
}
