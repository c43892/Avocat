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
        // 卡片分解增加道具使用能量
        public static int ItemUsagePerCard { get; set; } = 20;

        // 星之泪效果公式
        public static int StarTearsEffect(int dhp)
        {
            return dhp > 0 ? 20 + dhp / 3 : 0;
        }
    }
}
