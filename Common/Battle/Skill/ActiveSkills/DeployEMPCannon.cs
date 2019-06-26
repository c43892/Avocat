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
    public class DeployEMPCannon : ActiveSkill, IWithRange
    {
        public override string Name { get => "EMP"; }
        public override string DisplayName { get => "部署 EMP 炮台"; }

        // 能量消耗
        public override int EnergyCost { get; set; }
        public override string ActiveSkillType { get; } = "fireAt";

        //设置技能释放范围
        public int Range { get; set; }

        public override string SkillDescription { get { return "这是部署 EMP 炮台的技能描述这是部署 EMP 炮台的技能描述这是部署 EMP 炮台的技能描述"; } }

        // 是否部署加速炮台，该开关被符文影响
        public bool FastCannon { get; set; } = false;

        // 是否部署举行炮台，该开关被符文影响
        public bool BigCannon { get; set; } = false;

        // 部署炮台时销毁所有已存在的其它炮台，该开关被符文影响
        public bool DestroyPreviousOnes { get; set; } = false;

        // 主动释放
        public override void FireAt(int x, int y)
        {
            var bt = Battle as BattlePVE;
            if (DestroyPreviousOnes)
                Map.ForeachObjs<EMPCannon>((px, py, c) => bt.AddHP(c, -c.HP));

            var cannon = Configuration.Config(new EMPCannon(Map, FastCannon) { Team = Owner.Team });
            cannon.Team = Owner.Team;
            if (BigCannon)
            {
                cannon.ATK = Owner.ATK;
                cannon.MaxHP = Owner.HP;
                cannon.HP = cannon.MaxHP;
            }
            else
            {
                cannon.ATK = Owner.ATK * 2;
                cannon.MaxHP = Owner.HP * 2;
                cannon.HP = cannon.MaxHP;
            }

            Battle.AddWarriorAt(x, y, cannon);
        }
    }
}
