using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 创伤效果，不可治疗
    /// </summary>
    public class Untreatable : BuffCountDown
    {
        public Untreatable(int num)
            :base(num)
        {
        }
        public override string Name { get; } = "Untreatable";
        void OnBeforeAddHp(Warrior warrior, int dhp, Action<int> changeDhp)
        {
            if (warrior != Warrior || dhp <= 0)
                return;

            changeDhp(0);
        }

        public override void OnAttached()
        {
            Battle.BeforeAddHP += OnBeforeAddHp;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.BeforeAddHP -= OnBeforeAddHp;
            base.OnDetached();
        }
    }
}
