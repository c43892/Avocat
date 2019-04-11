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
    public TextMesh cardSR;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public static readonly Color ColorDefault = Color.white;
    public static readonly Color ColorSelected = Color.green;
    public static readonly Color ColorSelectedHead = Color.blue;

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
                cardSR.gameObject.SetActive(false);
            else
            {
                cardSR.gameObject.SetActive(true);
                cardSR.GetComponent<MeshRenderer>().sortingLayerID = sr.sortingLayerID;
                cardSR.GetComponent<MeshRenderer>().sortingOrder = sr.sortingOrder + 1;
                cardSR.text = card.Name;
            }
        }
    } BattleCard card = null;
}
