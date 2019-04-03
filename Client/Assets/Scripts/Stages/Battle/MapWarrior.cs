using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWarrior : MonoBehaviour
{
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
}
