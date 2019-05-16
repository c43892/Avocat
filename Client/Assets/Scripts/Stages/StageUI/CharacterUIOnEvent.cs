using Avocat;
using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class CharacterUIOnEvent 
{
    public static void SetupUIHandler(this BattleStage BattleStage, BattleRoomClient room) {
        var bt = BattleStage.Battle as BattlePVE;
        var aniPlayer = BattleStage.GetComponent<MapAniPlayer>();
        var characterui = BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>().CharacterInfoUI;
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

        bt.OnSetActionFlag += (warrior, isActionDone) =>
        {
            // 如果英雄完成回合则w动作显示技能框
            if (isActionDone)
            {
                BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>().SkillButtonUI.UpdateSkill(warrior);
                BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>().SkillButtonUI.UpdateSkillState((BattleStage.Battle as BattlePVE).Energy, warrior);
            }
        };

        // 回合结束清除左上角信息栏显示
        bt.OnActionDone += (int team) =>
        {
            BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>().CharacterInfoUI.gameObject.SetActive(false);
            BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>().obstacleUI.gameObject.SetActive(false);
        };

        // 角色死亡清除信息栏显示
        bt.OnWarriorDying += (warrior) =>
        {
            aniPlayer.Op(() => characterui.gameObject.SetActive(false));
        };
    }
}
