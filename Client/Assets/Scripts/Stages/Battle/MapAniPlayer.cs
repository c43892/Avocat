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

    List<KeyValuePair<IEnumerator, Action>> anis = new List<KeyValuePair<IEnumerator, Action>>();

    // 要播放的动画入队
    public void Add(IEnumerator routine, Action callback = null)
    {
        anis.Add(new KeyValuePair<IEnumerator, Action>(routine, callback));
        if (anis.Count == 1)
            StartCoroutine(StartPlaying());
    }

    // 插入指定动作
    public void AddOp(Action act)
    {
        Add(Op(act));
    }

    IEnumerator StartPlaying()
    {
        while (anis.Count > 0)
        {
            var ani = anis[0].Key;
            var cb = anis[0].Value;
            yield return StartCoroutine(ani);
            anis.RemoveAt(0);
            cb.SC();
        }
    }

    #region 构建不同动画

    // 执行指定动作
    IEnumerator Op(Action op)
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
    public IEnumerator MakeMovingOnPath(Transform tar, float velocity, float[] path)
    {
        yield return null;
        tar.localPosition = new Vector2(path[0], path[1]);

        var i = 2;
        while (i < path.Length)
        {
            var overNodesNum = RunOnPath(path, Time.deltaTime * velocity, tar.localPosition.x, tar.localPosition.y, i, out float x, out float y);
            i += overNodesNum * 2;
            tar.localPosition = new Vector2(x, y);
            yield return null;
        }
    }

    // 攻击动画
    public IEnumerator MakeAttacking(MapAvatar attacker, MapAvatar target)
    {
        var fx = attacker.transform.localPosition.x;
        var fy = attacker.transform.localPosition.y;
        var tx = target.transform.localPosition.x;
        var ty = target.transform.localPosition.y;

        yield return MakeMovingOnPath(attacker.transform, 20, new float[] { fx, fy, tx, ty });
        target.RefreshAttrs();
        yield return MakeMovingOnPath(attacker.transform, 20, new float[] { tx, ty, fx, fy });
    }

    // 角色死亡
    public IEnumerator MakeDying(MapAvatar avatar)
    {
        yield return null;
        avatar.transform.SetParent(null);
        Destroy(avatar.gameObject);
    }

    #endregion
}
