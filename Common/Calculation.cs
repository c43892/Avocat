using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 所有计算公式放在这里
    /// </summary>
    public class Calculation
    {
        // 基本伤害公式
        public static int CalcDamage(int basicAttack, int inc, int more, int damageDecFac)
        {
            var damage = basicAttack * (100 + inc) * (100 + more) / 10000;
            damage = damage - damage * damageDecFac / (100 + damageDecFac);
            return damage < 0 ? 0 : (int)damage;
        }

        // AXY 类型的基本伤害公式
        public static int CalcBasicAttackByAXY(Warrior attacker, ISkillWithAXY s)
        {
            return s.A + attacker.ATK * s.X / 100 + attacker.POW * s.Y / 100;
        }

        // 星之泪效果公式
        public static int StarTearsEffect(int dhp)
        {
            return dhp > 0 ? 20 + dhp * 3 / 10 : 0;
        }
    }
}
