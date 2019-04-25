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
        public override int EnergyCost { get; set; }

        public override string ActiveSkillType { get; } = "fireAt";

        // 主动释放
        public override IEnumerator FireAt(int x, int y)
        {
            var connon = new CannonEMP(Map) { Team = Warrior.Team, ATK = Warrior.ATK, MaxHP = Warrior.HP, HP = Warrior.HP };
            yield return Battle.AddWarriorAt(x, y, connon);
        }
    }
}
