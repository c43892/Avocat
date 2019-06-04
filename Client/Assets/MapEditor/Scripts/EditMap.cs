using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;
using UnityEngine.UI;
using Swift.Math;

public class EditMap : MonoBehaviour
{
    public BattleMap Map { get; set; }
    public MapTile MapTile;
    public Transform MapRoot;
    public StageOpsLayer CurrentOpLayer { get; private set; }
    public MapTile[,] Tiles { get; private set; }
    public List<MapTile> MapTilesList = new List<MapTile>();
    public bool IsMapCreated { get; set; }

    public void BuildMapGrids()
    {
        JudgeMapStatus();
        if (!IsMapCreated)
        {
            Map = new BattleMap(10, 6, (x, y) => TileType.Grass);
            Tiles = new MapTile[Map.Width, Map.Height];
            FC.For2(Map.Width, Map.Height, (x, y) =>
            {
                var tile = Instantiate(MapTile);
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/MapTile");
                tile.transform.SetParent(MapRoot);
                tile.X = x;
                tile.Y = y;
                tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                tile.gameObject.SetActive(true);
                Tiles[x, y] = tile;
                MapTilesList.Add(tile);
            });
            IsMapCreated = true;
        }
        else
        {
            Debug.Log("Map is already created!");
        }
    }

    public void DestroyMap()
    {
        JudgeMapStatus();
        if (IsMapCreated)
        {
            foreach (MapTile map in MapTilesList)
                DestroyImmediate(map.gameObject);
            MapTilesList.Clear();
            IsMapCreated = false;
        }
        else
        {
            Debug.Log("Map doesn't exist!");
        }
    }

    public void JudgeMapStatus() {
        if (MapTilesList != null && MapTilesList.Count > 0)
        {
            IsMapCreated = true;
        }
        else
        {
            IsMapCreated = false;
        }
    }
}
