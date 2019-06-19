using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using Spine.Unity;

public class PoniterIndicator : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public MeshRenderer Mesh;
    public void SetIdleAnimation(MapAvatar avater) {
        GameObject MapWarrior = avater.gameObject;
        skeletonAnimation.skeletonDataAsset = MapWarrior.transform.Find("_MapWarrior").GetComponent<SkeletonAnimation>().skeletonDataAsset;
        skeletonAnimation.Initialize(true);
        Mesh.sortingOrder = 8;
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }
}
