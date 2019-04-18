using Avocat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class BattleStageOnEvents
{
    public static void SetupEventHandler(this BattleStage BattleStage, BattleRoomClient room)
    {
        var bt = BattleStage.Battle as BattlePVE;

        bt.OnPlayerPrepared.Add((int player) =>
        {
            if (room.Battle.AllPrepared)
            {
                BattleStage.StartFighting();
            }
        });

        bt.OnAddHP.Add((warrior, dhp) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(warrior);
            avatar.RefreshAttrs();
        });

        bt.OnAddATK.Add((warrior, dATK) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(warrior);
            avatar.RefreshAttrs();
        });

        bt.OnAddES.Add((warrior, des) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(warrior);
            avatar.RefreshAttrs();
        });
    }
}
