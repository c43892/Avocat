using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.AStar;
using Swift.Math;
using System.Diagnostics;
using System.Collections;

namespace Avocat
{
    /// <summary>
    /// PVE 战斗房间
    /// </summary>
    public class BattlePVERoom : BattleRoom
    {
        public new BattlePVE Battle { get { return base.Battle as BattlePVE; } }

        public BattlePVERoom(BattlePVE bt)
            : base(bt)
        {
        }

        // 注册所有战斗消息
        public override void RegisterBattleMessageHandlers(IBattlemessageProvider bmp)
        {
            base.RegisterBattleMessageHandlers(bmp);

            bmp.HandleMsg("ExchangeBattleCards", (player, data) =>
            {
                var g1 = data.ReadInt();
                var n1 = data.ReadInt();
                var g2 = data.ReadInt();
                var n2 = data.ReadInt();

                return Battle.ExchangeBattleCards(g1, n1, g2, n2);
            });

            bmp.HandleMsg("FireActiveSkill", (player, data) =>
            {
                var warriorID = data.ReadInt();
                var skillName = data.ReadString();
                var warrior = Battle.Map.GetWarriorByID(warriorID);
                var skill = warrior.GetActiveSkillByName(skillName);
                return Battle.FireSkill(skill);
            });

            bmp.HandleMsg("FireActiveSkillAt", (player, data) =>
            {
                var warriorID = data.ReadInt();
                var skillName = data.ReadString();
                var x = data.ReadInt();
                var y = data.ReadInt();
                var warrior = Battle.Map.GetWarriorByID(warriorID);
                var skill = warrior.GetActiveSkillByName(skillName);
                return Battle.FireSkillAt(skill, x, y);
            });

            bmp.HandleMsg("UseItem2", (player, data) =>
            {
                var itemID = data.ReadInt();
                var targetID = data.ReadInt();
                var item = Battle.Map.GetItemByID(itemID);
                var target = Battle.Map.GetWarriorByID(targetID);
                return Battle.UseItem2(item as UsableItem, target);
            });
        }
    }
}
