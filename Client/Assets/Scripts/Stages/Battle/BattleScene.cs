using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using Spine.Unity;

public class BattleScene : MonoBehaviour
{
    public SkeletonAnimation attackSkeleton;
    public SkeletonAnimation targetSkeleton;
 
    public IEnumerator SetBattleAnimation(MapAvatar warrior, MapAvatar target) {
        gameObject.SetActive(true);
        attackSkeleton.skeletonDataAsset = warrior.gameObject.GetComponent<SkeletonAnimation>().skeletonDataAsset;
        targetSkeleton.skeletonDataAsset = target.gameObject.GetComponent<SkeletonAnimation>().skeletonDataAsset;
        attackSkeleton.Initialize(true);
        targetSkeleton.Initialize(true);
        targetSkeleton.skeleton.scaleX = -1;
        attackSkeleton.AnimationState.SetAnimation(0, "attack", false);
        yield return new WaitForSeconds(0.5f);
        targetSkeleton.AnimationState.SetAnimation(0, "hurt", false);
        yield return new WaitForSeconds(0.3f);
        targetSkeleton.AnimationState.SetAnimation(0, "idle", false);
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }

    public void WarriorDying(MapAvatar avatar) {
        var warriorAnimation = avatar.gameObject.GetComponent<SkeletonAnimation>();
        warriorAnimation.AnimationState.SetAnimation(0, "die", false);
    }
}
