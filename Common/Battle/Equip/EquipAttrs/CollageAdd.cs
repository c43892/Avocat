using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 回合开始前，获得 n 张额外的随机指令卡
    /// </summary>
    public class CollageAdd : EquipAttr
    {
        public int P0 { get; set; } // 卡片数量

        public override void OnPreparingBattle(Battle bt)
        {
            bt.BeforeStartNextRound2 += (int team) =>
            {
                if (team != Team)
                    return;

                // 添加随机指令卡
                var btpve = bt as BattlePVE;
                btpve.AddCards(btpve.GenNextCards(P0));
            };
        }
    }
}
