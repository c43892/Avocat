using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using Spine.Unity;

public class PoniterIndicator : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public void SetIdleAnimation(MapAvatar avater) {
        GameObject MapWarrior = avater.gameObject;
        skeletonAnimation.skeletonDataAsset = MapWarrior.GetComponent<SkeletonAnimation>().skeletonDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }
}
