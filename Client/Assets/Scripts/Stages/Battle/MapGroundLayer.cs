using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 统一由地表底下一层响应地图相关操作
/// </summary>
public class MapGroundLayer : MonoBehaviour
{
    public event Action<float, float> OnClicked;

    public event Action<float, float> OnStartDragging;
    public event Action<float, float, float, float> OnDragging;
    public event Action<float, float, float, float> OnEndDragging;

    public event Action<float> OnScaling;

    string opStatus = "default"; // default, down, pressing, dragging, scaling

    // 需要手动校正两个触点的 id
    int[] ptid = new int[] { -1, -1 };
    Vector2 pt0Pos = Vector2.zero;
    void AdjustPointerID()
    {
        if (Input.touchCount <= 1)
        {
            ptid[0] = 0;
            ptid[1] = -1;
            if (Input.touchCount == 1)
                pt0Pos = Input.touches[0].position;

            return;
        }

        if (ptid[1] < 0)
        {
            var dp0 = (Input.touches[0].position - pt0Pos).magnitude;
            var dp1 = (Input.touches[1].position - pt0Pos).magnitude;
            ptid[0] = dp0 < dp1 ? 0 : 1;
            ptid[1] = dp0 < dp1 ? 1 : 0;
        }
    }

    bool CheckPointerRelativePosition(out Vector2 v2, int pt = 0)
    {
        v2 = Vector2.zero;

        // 点在 ui 上就不处理了
        if (BattleStageUI.CheckGuiRaycastObjects())
            return false;

        var ray = Camera.main.ScreenPointToRay(Input.touchCount == 0 ? (Vector2)Input.mousePosition : Input.touches[ptid[pt]].position);
        var hitInfos = Physics2D.GetRayIntersectionAll(ray);
        if (hitInfos.Length > 0 && hitInfos[0].collider?.gameObject == gameObject)
        {
            v2 = hitInfos[0].point;
            return true;
        }

        return false;
    }

    Vector2 dragFrom;
    private void OnMouseDown()
    {
        AdjustPointerID();
        if (Input.touchCount > 1) return;
        if (!CheckPointerRelativePosition(out Vector2 pt)) return;

        opStatus = "down";
        dragFrom = pt;
    }

    private void OnMouseUp()
    {
        AdjustPointerID();
        if (opStatus == "down")
            OnClicked.SC(dragFrom.x, dragFrom.y);
        else if (opStatus == "dragging")
            OnEndDragging.SC(dragFrom.x, dragFrom.y, lastDraggingPos.x, lastDraggingPos.y);

        opStatus = "default";
    }

    Vector2 draggingOffset;
    Vector2 lastDraggingPos;
    private void OnMouseDrag()
    {
        AdjustPointerID();
        if (!CheckPointerRelativePosition(out Vector2 pt)) return;

        if (opStatus == "down")
        {
            if ((pt - dragFrom).sqrMagnitude <= 0.1f)
                return;

            opStatus = "dragging";
            OnStartDragging.SC(dragFrom.x, dragFrom.y);
            lastDraggingPos = dragFrom;
            draggingOffset = Vector2.zero;
        }
        else if (opStatus == "dragging")
        {
            var dPos = pt - lastDraggingPos;
            lastDraggingPos = pt;
            draggingOffset += dPos;
            OnDragging.SC(dragFrom.x, dragFrom.y, dragFrom.x + draggingOffset.x, dragFrom.y + draggingOffset.y);
        }
    }

    Vector2 centreFrom;
    private void Update()
    {
        AdjustPointerID();
        if (!CheckPointerRelativePosition(out Vector2 pt)) return;

        if (opStatus == "scaling")
        {
            if (Input.touchCount == 0)
            {
                OnEndDragging.SC(dragFrom.x, dragFrom.y, lastDraggingPos.x, lastDraggingPos.y);
                opStatus = "default";
            }
            else if (Input.touchCount == 1)
            {
                lastDraggingPos = pt;
                opStatus = "dragging";
            }
            else
            {
                if (!CheckPointerRelativePosition(out Vector2 pt1, 1)) return;

                var centreTo = (pt1 + pt) / 2;
                var dPos = centreTo - centreFrom;
                centreFrom = centreTo;
                draggingOffset += dPos;
                lastDraggingPos = pt;
                OnDragging.SC(dragFrom.x, dragFrom.y, dragFrom.x + draggingOffset.x, dragFrom.y + draggingOffset.y);
            }
        }
        else if (Input.touchCount == 2)
        {
            if (!CheckPointerRelativePosition(out Vector2 pt1, 1)) return;
            if (opStatus != "dragging")
            {
                lastDraggingPos = dragFrom = pt;
                draggingOffset = Vector2.zero;
                OnStartDragging.SC(dragFrom.x, dragFrom.y);
            }

            centreFrom = (pt1 + pt) / 2;
            opStatus = "scaling";
        }
    }
}
