using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;

[Serializable]
public class MapData
{
    public int X { get; set; }
    public int Y { get; set; }
    public TileType Type{ get; set; }
    public int MaterialSortingOrder { get; set; }
    public int RespawnSortingOrder { get; set; }
    public bool RespawnForChamp;
    public bool RespawnForEnemy;

    public MapData()
    {
    }

    public MapData(int X, int Y, TileType Type, int MaterialSortingOrder, int RespawnSortingOrder)
    {
        this.X = X;
        this.Y = Y;
        this.Type = Type;
        this.MaterialSortingOrder = MaterialSortingOrder;
        this.RespawnSortingOrder = RespawnSortingOrder;
    }
}
