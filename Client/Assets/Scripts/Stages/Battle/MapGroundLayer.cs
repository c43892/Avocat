using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;

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

    Vector2 PointerRelativePosition
    {
        get
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hitInfo = Physics2D.GetRayIntersection(ray, float.MaxValue, LayerMask.GetMask("MapGroundLayer"));
            if (hitInfo.collider != null)
            {
                var pos = transform.worldToLocalMatrix.MultiplyPoint(hitInfo.point);
                return new Vector2(pos.x * Area.width, -pos.y * Area.height);
            }
            else
                return Vector2.zero;
        }
    }

    private void OnMouseDown()
    {
        switch (opStatus)
        {
            case "default":
                opStatus = "down";
                fromPos = PointerRelativePosition;
                draggingLastPos = fromPos;
                break;
        }
    }

    private void OnMouseUp()
    {
        var currentPos = PointerRelativePosition;
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
        var currentPos = PointerRelativePosition;
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
