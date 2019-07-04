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
    public class LuoLiSiRune1 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as LuoLiSi;
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
    public class LuoLiSiRune2 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.GetActiveSkill<DeployEMPCannon>().FastCannon = true;
        }
    }

    /// <summary>
    /// 匠心独运
    /// 由匠心产生的能量翻倍
    /// </summary>
    public class LuoLiSiRune3 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            var s = h.GetBuffSkill<ArtisanSpirit>();
            s.EffectFactor = 2;
        }
    }

    /// <summary>
    /// 废物利用
    /// 炮台销毁时获得能量
    /// </summary>
    public class LuoLiSiRune4 : Rune
    {
        public override void OnPreparingBattle()
        {
            var h = Owner as LuoLiSi;
            Debug.Assert(h != null, "only available for LuoLiSi");

            h.SetupSkills(new GainENOnEMPDestroyed(Owner));
        }
    }
}
