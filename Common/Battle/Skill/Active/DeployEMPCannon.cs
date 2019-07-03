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
    public class DeployEMPCannon : ActiveSkill, ISkillWithRange, ISkillWithPosSel, ISkillWithTargetFilter
    {
        public override string ID { get => "EMP"; }
        public DeployEMPCannon(Warrior owner) : base(owner) { }
        public int Range { get; set; } //设置技能释放范围
        public bool FastCannon { get; set; } = false; // 是否部署加速炮台，该开关被符文影响
        public bool BigCannon { get; set; } = false; // 是否部署举行炮台，该开关被符文影响
        public bool DestroyPreviousOnes { get; set; } = false; // 部署炮台时销毁所有已存在的其它炮台，该开关被符文影响
        public int TX { get; set; } // 目标坐标 x
        public int TY { get; set; } // 目标坐标 y

        // 只能放在空位
        public bool TargetFilter(BattleMapObj target) => target == null;

        // 主动释放
        public override void Fire()
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

            Battle.AddWarriorAt(TX, TY, cannon);
        }
    }
}
