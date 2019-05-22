using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using Spine.Unity;

public class BattleScene : MonoBehaviour
{
    public SkeletonAnimation champSkeleton;
    public SkeletonAnimation enemySkeleton;
 
    public IEnumerator SetBattleAnimation(MapAvatar warrior, MapAvatar target) {
        gameObject.SetActive(true);
        var attackerAvatar = warrior.transform.Find("_MapWarrior");
        var targetAvatar = target.transform.Find("_MapWarrior");

        if (warrior.Warrior.Team==1) // 如果攻击方是己方
        {
            champSkeleton.skeletonDataAsset = attackerAvatar.GetComponent<SkeletonAnimation>().skeletonDataAsset;
            enemySkeleton.skeletonDataAsset = targetAvatar.GetComponent<SkeletonAnimation>().skeletonDataAsset;
            champSkeleton.Initialize(true);
            enemySkeleton.Initialize(true);
            enemySkeleton.skeleton.scaleX = -1;
            champSkeleton.AnimationState.SetAnimation(0, "attack", false);
            yield return new WaitForSeconds(0.5f);
            enemySkeleton.AnimationState.SetAnimation(0, "hurt", false);
            yield return new WaitForSeconds(0.3f);
            enemySkeleton.AnimationState.SetAnimation(0, "idle", false);
            yield return new WaitForSeconds(0.7f);
        }
        else { //如果攻击方是敌军
            enemySkeleton.skeletonDataAsset = attackerAvatar.GetComponent<SkeletonAnimation>().skeletonDataAsset;
            champSkeleton.skeletonDataAsset = targetAvatar.GetComponent<SkeletonAnimation>().skeletonDataAsset;
            champSkeleton.Initialize(true);
            enemySkeleton.Initialize(true);
            enemySkeleton.skeleton.scaleX = -1;
            enemySkeleton.AnimationState.SetAnimation(0, "attack", false);
            yield return new WaitForSeconds(0.5f);
            champSkeleton.AnimationState.SetAnimation(0, "hurt", false);
            yield return new WaitForSeconds(0.3f);
            champSkeleton.AnimationState.SetAnimation(0, "idle", false);
            yield return new WaitForSeconds(0.7f);
        }        
        gameObject.SetActive(false);
    }

    public void WarriorDying(MapAvatar avatar) {
        var warriorAnimation = avatar.transform.Find("_MapWarrior").GetComponent<SkeletonAnimation>();
        warriorAnimation.AnimationState.SetAnimation(0, "die", false);
    }

}
