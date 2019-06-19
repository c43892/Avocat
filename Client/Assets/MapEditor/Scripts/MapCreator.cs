using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using System;
using UnityEngine.UI;
using Swift.Math;
using LitJson;
using System.IO;

public class MapCreator : MonoBehaviour
{
    public SpriteRenderer BackGround;
    public TileType TileType;
    public BattleMap Map { get; set; }
    public MapTile MapTile;
    public Transform MapRoot;
    public MapTile[,] Tiles { get; private set; }
    public List<MapTile> MapTilesList = new List<MapTile>();
    public bool IsMapCreated { get; set; }
    public List<MapData> MapInfo = new List<MapData>();
    public MapDataCollection MapList = new MapDataCollection();
    public int MapNumber;
    public bool ChooseRespawn;
    public RespawnType respawnType { get; set; }

    public  Sprite Sprite{
        get { return BackGround.sprite; }
        set { BackGround.sprite = value; }
    }

    public enum RespawnType
    {
        Hero,
        Enemy,
        None
    }

    public void BuildMapGrids()
    {
        JudgeMapStatus();
        if (!IsMapCreated)
        {
            Map = new BattleMap(10, 6, (x, y) => TileType.None);
            Tiles = new MapTile[Map.Width, Map.Height];
            FC.For2(Map.Width, Map.Height, (x, y) =>
            {
                var tile = Instantiate(MapTile);
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/MapTile");
                tile.transform.SetParent(MapRoot);
                tile.X = x;
                tile.Y = y;
                tile.transform.Find("Material").GetComponent<SpriteRenderer>().sortingOrder = y + 2;
                tile.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>().sortingOrder = y + 3;
                tile.gameObject.SetActive(true);
                Tiles[x, y] = tile;
                MapTilesList.Add(tile);
                // var mapData = tile.GetComponent<MapTile>().MapData;

                // MapData没有new之前就传递是值传递
                tile.GetComponent<MapTile>().MapData = new MapData(x, y, TileType.None, y + 2);
                var mapData = tile.GetComponent<MapTile>().MapData;
                //mapData.X = x;
                //mapData.Y = y;
                //mapData.SortingOrder = y + 2;
                MapInfo.Add(mapData);
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

    public void JudgeMapStatus()
    {
        if (MapTilesList != null && MapTilesList.Count > 0)
        {
            IsMapCreated = true;
        }
        else
        {
            IsMapCreated = false;
        }
    }

    public void SaveToJson()
    {
        JsonData MapType;
        if (MapInfo.Count == 0 )
            Debug.Log("Don't have inforamtion of map");
        else
        {
            MapList.MapData = MapInfo;
            MapType = JsonMapper.ToJson(MapList);
          //  Debug.Log(MapType.ToString());
            var fileName = Path.Combine(Application.dataPath,"Map", "MapInfo.json");
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, MapType.ToString());
            Debug.Log("Map information is saved!");
        }  
    }

    public void ShowRespawnGrid()
    {
        var Maptile = GameObject.FindGameObjectsWithTag("MapTile");
        FC.For(Maptile.Length, (i) =>
        {
            Maptile[i].transform.Find("RespawnPlace").gameObject.SetActive(true);
        });
    }

    public void HideRespawnGrid()
    {
        var Maptile = GameObject.FindGameObjectsWithTag("MapTile");
        FC.For(Maptile.Length, (i) =>
        {
            Maptile[i].transform.Find("RespawnPlace").gameObject.SetActive(false);
        });
    }
}
