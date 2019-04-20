using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;

namespace Avocat
{
    /// <summary>
    /// EMP 炮台
    /// 部署 EMP 炮台
    /// </summary>
    public class DeployEMPConnon : ActiveSkill
    {
        public override string Name { get => "EMP 炮台"; }

        // 能量消耗
        public override int EnergyCost { get => 30; }

        // 主动释放
        public override IEnumerator Fire()
        {
            var foundPos = false;
            Warrior.GetPosInMap(out int cx, out int cy);
            int px = 0;
            int py = 0;
            FC.For2(-1, 2, -1, 2, (x, y) =>
            {
                if (!Map.BlockedAt(cx + x, cy + y))
                {
                    foundPos = true;
                    px = cx + x;
                    py = cy + y;
                }
            }, () => !foundPos);

            if (foundPos)
            {
                var connon = new CannonEMP(Map, Warrior.HP) { Team = Warrior.Team, ATK = Warrior.ATK };
                yield return Battle.AddWarriorAt(px, py, connon);
            }
        }
    }
}
