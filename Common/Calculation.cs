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
        // 攻击卡牌效果百分比
        public static int ATKCardEffect { get; set; } = 15;

        // 盾卡牌效果百分比
        public static int ESCardEffect { get; set; } = 25;

        // 每张卡牌充能量
        public static int ENCardEffect { get; set; } = 15;

        // 计算攻击数值
        public static void CalcAttack(Warrior attacker, Warrior target, List<string> attackFlags, out int dhp, out int des)
        {
            if (attackFlags.Contains("ChaoticDamage"))
            {
                // 混乱攻击
                dhp = attacker.ATK / 2 + attacker.POW / 2;
                des = 0;
            }
            else if (attackFlags.Contains("MagicalDamage"))
            {
                // 魔法攻击
                var attack = attacker.ATK / 2 + attacker.POW;
                var damageDec = target.RES / (100 + target.RES);
                des = attack >= target.ES ? -target.ES : -attack;
                dhp = attack >= target.ES ? -(attack - target.ES) : 0;
            }
            else
            {
                // 物理攻击
                var attack = attacker.ATK + attacker.POW / 2;
                var damageDec = target.ARM / (100 + target.ARM);
                des = attack >= target.ES ? -target.ES : -attack;
                dhp = attack >= target.ES ? -(attack - target.ES) : 0;
            }
        }

        // 卡片分解增加道具使用能量
        public static int ItemUsagePerCard { get; set; } = 20;

        // 星之泪效果公式
        public static int StarTearsEffect(int dhp)
        {
            return dhp > 0 ? 20 + dhp / 3 : 0;
        }
    }
}
