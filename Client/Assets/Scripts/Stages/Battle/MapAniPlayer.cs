using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;
using UnityEngine;
using System.Collections;

/// <summary>
/// 缓存所有待播放的地图动画，并逐一播放
/// </summary>
public class MapAniPlayer : MonoBehaviour
{
    BattleStage BattleStage
    {
        get
        {
            if (stage == null)
                stage = GetComponent<BattleStage>();

            return stage;
        }
    } BattleStage stage;

    // 动画播放速率
    public float AnimationTimeScaleFactor { get; set; } = 1;

    #region 动画队列
    
    class AniNodeInfo
    {
        public IEnumerator Ani;
        public Action OnEnded;
    }

    Queue<AniNodeInfo> anis = new Queue<AniNodeInfo>();

    // 添加动画到动画队列
    AniNodeInfo tail = null;
    MapAniPlayer Add(IEnumerator ani, Action onEnded = null)
    {
        tail = new AniNodeInfo() { Ani = ani, OnEnded = onEnded };
        anis.Enqueue(tail);
        if (anis.Count == 1)
            StartCoroutine(PlayAnis());

        return this;
    }

    IEnumerator PlayAnis()
    {
        while (anis.Count > 0)
        {
            var pair = anis.Peek();
            var ani = pair.Ani;
            yield return ani;
            var onEnded = pair.OnEnded;
            onEnded?.Invoke();
            anis.Dequeue();
        }

        tail = null;
    }

    #endregion

    #region 构建不同动画

    public MapAniPlayer OnEnded(Action onEnded)
    {
        Debug.Assert(anis.Count > 0 && tail != null, "current animation is null");
        tail.OnEnded = onEnded;
        return this;
    }

    // 执行指定动作
    public MapAniPlayer Op(Action op) => Add(OpImpl(op));
    IEnumerator OpImpl(Action op)
    {
        yield return null;
        op();
    }

    // 计算沿路径移动位置，返回值表示越过了几个中间节点
    int RunOnPath(float[] path, float dist, float fx, float fy, int toNode, out float x, out float y)
    {
        x = 0;
        y = 0;
        var skippedNum = 0;

        while (dist > 0 && toNode < path.Length)
        {
            var tx = path[toNode];
            var ty = path[toNode + 1];
            var dx = tx - fx;
            var dy = ty - fy;

            var dd = Mathf.Sqrt(dx * dx + dy * dy);
            if (dd < dist)
            {
                // 越过下一个节点
                dist -= dd;
                toNode += 2;
                fx = tx;
                fy = ty;
                skippedNum++;

                x = tx;
                y = ty;
            }
            else
            {
                x = fx + dx * dist / dd;
                y = fy + dy * dist / dd;
                dist = 0;
            }
        }

        return skippedNum;
    }

    // 沿路径移动动画
    public MapAniPlayer MakeMovingOnPath(Transform tar, float velocity, float[] path, Action onPosChanged = null) => Add(MakeMovingOnPathImpl(tar, velocity, path, onPosChanged));
    IEnumerator MakeMovingOnPathImpl(Transform tar, float velocity, float[] path, Action onPosChanged = null)
    {
        yield return null;

        var i = 0;
        while (i < path.Length)
        {
            var overNodesNum = RunOnPath(path, Time.deltaTime * velocity * AnimationTimeScaleFactor, tar.localPosition.x, tar.localPosition.y, i, out float x, out float y);
            tar.localPosition = new Vector2(x, y);

            if (overNodesNum > 0)
            {
                i += overNodesNum * 2;
                onPosChanged.SC();
            }

            yield return null;
        }
    }

    // 攻击动画1
    public MapAniPlayer MakeAttacking1(MonoBehaviour attacker, MonoBehaviour target) => Add(MakeAttacking1Impl(attacker, target));
    IEnumerator MakeAttacking1Impl(MonoBehaviour attacker, MonoBehaviour target)
    {
        var fx = attacker.transform.localPosition.x;
        var fy = attacker.transform.localPosition.y;
        var tx = target.transform.localPosition.x;
        var ty = target.transform.localPosition.y;

        yield return MakeMovingOnPathImpl(attacker.transform, 20, new float[] { fx, fy, tx, ty });
    }

    // 攻击动画2
    public MapAniPlayer MakeAttacking2(MonoBehaviour attacker, MonoBehaviour target) => Add(MakeAttacking2Impl(attacker, target));
    IEnumerator MakeAttacking2Impl(MonoBehaviour attacker, MonoBehaviour target)
    {
        var fx = attacker.transform.localPosition.x;
        var fy = attacker.transform.localPosition.y;
        var tx = target.transform.localPosition.x;
        var ty = target.transform.localPosition.y;

        yield return MakeMovingOnPathImpl(attacker.transform, 20, new float[] { fx, fy, tx, ty });
        yield return MakeMovingOnPathImpl(attacker.transform, 20, new float[] { tx, ty, fx, fy });
    }

    // 角色死亡
    public MapAniPlayer MakeDying(MapAvatar avatar) => Add(MakeDyingImpl(avatar));
    IEnumerator MakeDyingImpl(MapAvatar avatar)
    {
        yield return null;
    }

    #endregion
}
