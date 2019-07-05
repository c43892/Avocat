using Avocat;
using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;


static class CharacterUIOnEvent 
{
    public static void SetupUIHandler(this BattleStage BattleStage, BattleRoomClient room) {
        var bt = BattleStage.Battle as BattlePVE;
        var aniPlayer = BattleStage.GetComponent<MapAniPlayer>();
        var stageUI = BattleStage.BattleStageUIRoot.GetComponent<BattleStageUI>();
        var characterui = stageUI.CharacterInfoUI;
        var skillButtonUI = stageUI.SkillButtonUI;

        bt.OnAddHP += (warrior, dhp) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(warrior));
        };

        //bt.OnAddHP += (warrior, dhp) =>
        //{
        //    aniPlayer.Op(() => skillButtonUI.UpdateSkill(warrior));
        //};

        bt.OnWarriorAttack += (warrior,target,skill, flags) =>
        {
            aniPlayer.Op(() => characterui.UpdateWarriorInfo(target));       
        };

        bt.OnWarriorAttack += (warrior, target, skill, flags) =>
        {
            aniPlayer.Op(() => skillButtonUI.UpdateSkillState(bt.Energy, (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior));
        };

        //bt.AfterAttack += (warrior, target, skill, flags) =>
        //{
        //    aniPlayer.Op(() => characterui.UpdateWarriorInfo((BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior));
        //};

        //bt.AfterAttack += (warrior, target, skill, flags) =>
        //{
        //    aniPlayer.Op(() => skillButtonUI.UpdateSkillState(bt.Energy, (BattleStage.CurrentOpLayer as InBattleOps)?.CurrentSelWarrior));
        //};

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
            // 如果英雄完成回合则动作显示技能框
            if (isActionDone)
                stageUI.SkillButtonUI.UpdateSkillState((BattleStage.Battle as BattlePVE).Energy, warrior);
        };

        // 回合结束清除左上角信息栏显示
        bt.OnActionDone += (int team) =>
        {
            stageUI.CharacterInfoUI.gameObject.SetActive(false);
            stageUI.obstacleUI.gameObject.SetActive(false);
        };

        // 角色死亡清除信息栏显示
        bt.OnWarriorDying += (warrior) =>
        {
            aniPlayer.Op(() => characterui.gameObject.SetActive(false));
        };
    }
}
