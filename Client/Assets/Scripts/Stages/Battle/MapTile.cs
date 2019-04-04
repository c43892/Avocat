using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Swift;

/// <summary>
/// 地块对象，负责场景内的地块基本交互逻辑
/// </summary>
public class MapTile : MonoBehaviour
{
    SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            sr.color = selected ? Color.green : Color.white;
        }
    } bool selected = false;

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
}
