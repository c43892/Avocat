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

    Vector2 fromPos;
    string opStatus = "default"; // default, down, pressing, dragging

    public Rect Area
    {
        get
        {
            return area;
        }
        set
        {
            area = value;
            transform.localPosition = new Vector3(area.x, area.y);
            transform.localScale = new Vector3(area.width, area.height, 1);
        }
    } Rect area;

    bool CheckPointerRelativePosition(out Vector2 v2)
    {
        v2 = Vector2.zero;

        // 点在 ui 上就不处理了
        if (BattleStageUI.CheckGuiRaycastObjects())
            return false;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitInfos = Physics2D.GetRayIntersectionAll(ray);
        if (hitInfos.Length > 0 && hitInfos[0].collider?.gameObject == gameObject)
        {
            var pos = transform.worldToLocalMatrix.MultiplyPoint(hitInfos[0].point);
            v2 = new Vector2(pos.x * Area.width, -pos.y * Area.height);
            return true;
        }

        return false;
    }

    private void OnMouseDown()
    {
        switch (opStatus)
        {
            case "default":
                if (CheckPointerRelativePosition(out fromPos))
                {
                    opStatus = "down";
                    draggingLastPos = fromPos;
                }
                break;
        }
    }

    private void OnMouseUp()
    {
        if (!CheckPointerRelativePosition(out var currentPos))
            return;

        switch (opStatus)
        {
            case "down":
                opStatus = "default";
                OnClicked.SC(fromPos.x, fromPos.y);
                break;
            case "dragging":
                opStatus = "default";
                OnEndDragging.SC(fromPos.x, fromPos.y, currentPos.x, currentPos.y);
                break;
        }
    }

    Vector2 draggingLastPos;
    private void OnMouseDrag()
    {
        if (!CheckPointerRelativePosition(out var currentPos))
            return;

        switch (opStatus)
        {
            case "down":
                if ((currentPos - draggingLastPos).magnitude < 0.1f)
                    return;

                opStatus = "dragging";
                OnStartDragging.SC(currentPos.x, currentPos.y);
                OnDragging.SC(fromPos.x, fromPos.y, currentPos.x, currentPos.y);
                break;
            case "dragging":
                if ((currentPos - draggingLastPos).magnitude < 0.1f)
                    return;

                draggingLastPos = currentPos;
                OnDragging.SC(fromPos.x, fromPos.y, currentPos.x, currentPos.y);
                break;
        }
    }
}
