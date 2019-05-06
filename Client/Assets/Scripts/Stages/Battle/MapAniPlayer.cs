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

    Queue<KeyValuePair<IEnumerator, Action>> anis = new Queue<KeyValuePair<IEnumerator, Action>>();

    // 添加动画到动画队列
    public void Add(IEnumerator ani, Action onEnded = null)
    {
        anis.Enqueue(new KeyValuePair<IEnumerator, Action>(ani, onEnded));

        if (anis.Count == 1)
            StartCoroutine(PlayAnis());
    }

    IEnumerator PlayAnis()
    {
        while (anis.Count > 0)
        {
            var pair = anis.Peek();
            var ani = pair.Key;
            var onEnded = pair.Value;
            yield return ani;
            onEnded?.Invoke();
            anis.Dequeue();
        }
    }

    #endregion

    #region 构建不同动画

    // 执行指定动作
    public IEnumerator Op(Action op)
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
    public IEnumerator MakeMovingOnPath(Transform tar, float velocity, float[] path, Action onPosChanged = null)
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
    public IEnumerator MakeAttacking1(MonoBehaviour attacker, MonoBehaviour target)
    {
        var fx = attacker.transform.localPosition.x;
        var fy = attacker.transform.localPosition.y;
        var tx = target.transform.localPosition.x;
        var ty = target.transform.localPosition.y;

        yield return MakeMovingOnPath(attacker.transform, 20, new float[] { fx, fy, tx, ty });
    }

    // 攻击动画2
    public IEnumerator MakeAttacking2(MonoBehaviour attacker, MonoBehaviour target)
    {
        var fx = attacker.transform.localPosition.x;
        var fy = attacker.transform.localPosition.y;
        var tx = target.transform.localPosition.x;
        var ty = target.transform.localPosition.y;

        yield return MakeMovingOnPath(attacker.transform, 20, new float[] { fx, fy, tx, ty });
        yield return MakeMovingOnPath(attacker.transform, 20, new float[] { tx, ty, fx, fy });
    }

    // 角色死亡
    public IEnumerator MakeDying(MapAvatar avatar)
    {
        yield return null;
    }

    #endregion
}
