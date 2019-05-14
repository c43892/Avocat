using Avocat;
using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class BattleStageOnCharacterUI 
{
    public static void SetupUIHandler(this BattleStage BattleStage, BattleRoomClient room) {
        var bt = BattleStage.Battle as BattlePVE;
        var aniPlayer = BattleStage.GetComponent<MapAniPlayer>();
        var characterui = BattleStage.characterUI.GetComponent<CharacterInfoUI>();
        bt.OnAddHP += (warrior, dhp) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(warrior));
        };

        bt.OnAddES += (warrior, des) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(warrior));
        };

        bt.OnAddATK += (warrior, dATK) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(warrior));
        };

        bt.OnTransfrom += (warrior, state) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(warrior));
        };

        //room.Battle.OnActionDone += (int team) =>
        //{
        //    var op = BattleStage.CurrentOpLayer as InBattleOps;
        //    if(op.CurrentSelWarrior!=null)
        //        aniPlayer.Op(() => characterui.UpdateWarriorProperty(op.CurrentSelWarrior));
        //};
    }
}
