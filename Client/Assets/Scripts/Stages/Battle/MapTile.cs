using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Swift;
using Avocat;

/// <summary>
/// 地块对象，负责场景内的地块基本交互逻辑
/// </summary>
public class MapTile : MonoBehaviour
{
    SpriteRenderer sr;
    public SpriteRenderer CardSR;
    public SpriteRenderer DirSR;
    public MapData MapData;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public static readonly Color ColorDefault = new Color(1, 1, 1, 0.25f);
    public static readonly Color ColorSelected = Color.green;
    public static readonly Color ColorSelectedHead = Color.blue;

    public void SetDir(int dx, int dy)
    {
        var angle = 0;
        if (dx == 0 && dy == 0)
        {
            //DirSR.gameObject.SetActive(false);
            angle = 180;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/End");
        }

        else if (dx == 0 && dy > 0)
        {
            angle = 0;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Straight");
        }

        else if (dx == 0 && dy < 0)
        {
            angle = 0;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Straight");
        }

        else if (dx < 0 && dy == 0)
        {
            angle = 90;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Straight");
        }
        else if (dx > 0 && dy == 0)
        {
            angle = 90;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Straight");
        }
            DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // 设定头节点的图案样式
    public void SetHeadDir(int dx, int dy)
    {
        var angle = 0;
        if (dx == 0 && dy > 0)
        {
            angle = -90;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/End");
        }

        else if (dx == 0 && dy < 0)
        {
            angle = 90;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/End");
        }

        else if (dx < 0 && dy == 0)
        {
            angle = 0;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/End");
        }
        else if (dx > 0 && dy == 0)
        {
            angle = 180;
            DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/End");
        }
        DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    public Color Color
    {
        get
        {
            return sr.color;
        }
        set
        {
            sr.color = value;
        }
    }

    public int X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
            transform.localPosition = new Vector3(x, y);
        }
    } int x;

    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
            transform.localPosition = new Vector3(x, y);
        }
    } int y;

    public bool IsShowClickFrame
    {
        get
        {
            return transform.Find("ClickFrame").gameObject.activeSelf ? true : false;
        }
        set
        {
            transform.Find("ClickFrame").gameObject.SetActive(value);
        }
    }

    public BattleCard Card
    {
        set
        {
            card = value;
            if (card == null)
            {
                CardSR.gameObject.SetActive(false);
                DirSR.gameObject.SetActive(false);
            }
            else
            {
                DirSR.sortingLayerID = sr.sortingLayerID;
                DirSR.sortingOrder = sr.sortingOrder + 2;
                DirSR.gameObject.SetActive(true);

                CardSR.gameObject.SetActive(true);
                CardSR.sortingLayerID = sr.sortingLayerID;
                CardSR.sortingOrder = sr.sortingOrder + 3;
                CardSR.sprite = Resources.Load<Sprite>("UI/CardType/" + card.Name);
            }
        }
    } BattleCard card = null;


    public void JudgeCorner(MapTile secondTailTile, MapTile tailTile, MapTile newTile)
    {
        if (secondTailTile != null && tailTile != null && newTile != null)
        {
            var x1 = newTile.X - tailTile.X;
            var x2 = tailTile.X - secondTailTile.X;
            var y1 = newTile.Y - tailTile.Y;
            var y2 = tailTile.Y - secondTailTile.Y;
            var angle = 0;
            if ((y2 < 0 && x2 == 0) && (x1 > 0 && y1 == 0) || (y2 == 0 && x2 < 0) && (x1 == 0 && y1 > 0)) // "LeftUp"
            {
                tailTile.DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Corner");
                angle = 180;
                tailTile.DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else if ((x1 > 0 && y1 == 0) && (x2 == 0 && y2 > 0) || (x1 == 0 && y1 < 0) && (x2 < 0 && y2 == 0)) // "LeftDown"
            {
                tailTile.DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Corner");
                angle = 90;
                tailTile.DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else if ((x1 == 0 && y1 < 0) && (x2 > 0 && y2 == 0) || (x1 < 0 && y1 == 0) && (x2 == 0 && y2 > 0)) // "RightDown"
            {
                tailTile.DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Corner");
                angle = 0;
                tailTile.DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else if ((x1 < 0 && y1 == 0) && (x2 == 0 && y2 < 0) || (x1 ==0 && y1 > 0) && (x2 >0 && y2 == 0)) // "RightUp"
            {
                tailTile.DirSR.sprite = Resources.Load<Sprite>("UI/PathIcon/Corner");
                angle = -90;
                tailTile.DirSR.transform.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
