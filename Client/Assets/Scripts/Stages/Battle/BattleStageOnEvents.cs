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
                BattleStage.StartFighting();
        });

        bt.OnAddHP.Add((warrior, dhp) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddATK.Add((warrior, dATK) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddES.Add((warrior, des) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnTransfrom.Add((warrior, state) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddWarrior.Add((x, y, warrior) =>
        {
            BattleStage.CreateWarriorAvatar(x, y, warrior);
        });
    }
}
