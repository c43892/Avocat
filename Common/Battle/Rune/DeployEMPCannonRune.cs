using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 巨型炮台
    /// 炮台拥有翻倍的属性；洛里斯同时只能拥有1个炮台
    /// </summary>
    public class DeployEMPCannonRune1 : Rune
    {
        public DeployEMPCannonRune1()
        {
            DisplayName = "巨型炮台";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            var s = h.GetActiveSkill<DeployEMPCannon>();
            s.BigCannon = true;
            s.DestroyPreviousOnes = true;
        }
    }

    /// <summary>
    /// 加速炮台
    /// 炮台每回合能够进行1次额外的轰击；炮台在每次轰击后损失50%最大生命值
    /// </summary>
    public class DeployEMPCannonRune2 : Rune
    {
        public DeployEMPCannonRune2()
        {
            DisplayName = "加速炮台";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.GetActiveSkill<DeployEMPCannon>().FastCannon = true;
        }
    }

    /// <summary>
    /// 废物利用
    /// 炮台销毁时获得能量
    /// </summary>
    public class DeployEMPCannonRune3 : Rune
    {
        public DeployEMPCannonRune3()
        {
            DisplayName = "废物利用";
        }

        public override void OnPreparingBattle(Hero hero)
        {
            var h = hero as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.AddSkill(new GainENOnEMPDestroyed());
        }
    }
}
