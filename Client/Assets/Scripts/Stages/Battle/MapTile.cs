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
    public TextMesh CardSR;
    public TextMesh DirSR;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public static readonly Color ColorDefault = Color.white;
    public static readonly Color ColorSelected = Color.green;
    public static readonly Color ColorSelectedHead = Color.blue;

    public void SetDir(int dx, int dy)
    {
        var angle = 0;
        if (dx == 0 && dy == 0)
            DirSR.gameObject.SetActive(false);
        else if (dx == 0 && dy > 0)
            angle = 90;
        else if (dx == 0 && dy < 0)
            angle = -90;
        else if (dx < 0 && dy == 0)
            angle = 180;

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
                DirSR.GetComponent<MeshRenderer>().sortingLayerID = sr.sortingLayerID;
                DirSR.GetComponent<MeshRenderer>().sortingOrder = sr.sortingOrder + 1;
                DirSR.gameObject.SetActive(true);

                CardSR.gameObject.SetActive(true);
                CardSR.GetComponent<MeshRenderer>().sortingLayerID = sr.sortingLayerID;
                CardSR.GetComponent<MeshRenderer>().sortingOrder = sr.sortingOrder + 2;
                CardSR.text = card.Name;
            }
        }
    } BattleCard card = null;
}
