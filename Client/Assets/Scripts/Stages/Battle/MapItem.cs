﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

public class MapItem : MonoBehaviour
{
    public BattleStage BattleStage { get; set; }
    public Vector2 CenterOffset = new Vector2(0.5f, 0.5f);
    public SpriteRenderer sprite;
    public TextMesh NameText;

    // 对应的角色
    public BattleMapObj Item { get; set; }

    public void RefreshAttrs()
    {
      //  NameText.text = Item.DisplayName;
      //  Color = ColorDefault;
        sprite.sprite = Resources.Load<Sprite>("UI/MapItem/" + Item.ID);
        sprite.sortingOrder = BattleStage.Map.Height +2;
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
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    }
    int x;

    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    }
    int y;

    public static readonly Color ColorDefault = Color.black;
    public static readonly Color ColorSelected = Color.blue;

    public Color Color
    {
        get
        {
            return transform.Find("Name").Find("Name").GetComponent<TextMesh>().color;
        }
        set
        {
            transform.Find("Name").Find("Name").GetComponent<TextMesh>().color = value;
        }
    }
}
