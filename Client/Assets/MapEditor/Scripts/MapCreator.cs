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
using UnityEngine.SceneManagement;

public class MapCreator : MonoBehaviour
{
    public SpriteRenderer BackGround;
    public TileType TileType;
    public BattleMap Map { get; set; }
    public MapTile MapTile;
    public Transform MapRoot;
    public MapTile[,] Tiles { get; private set; }
    [HideInInspector]
    public List<MapTile> MapTilesList = new List<MapTile>();
    public bool IsMapCreated { get; set; }
    public List<string> StoredMap = new List<string>();
    public int MapIndex;
    public List<Transform> MapTileTransform = new List<Transform>();
    public bool isNewMap;

    // 储存地图信息
    [HideInInspector]
    public List<MapData> MapInfo = new List<MapData>();
    [HideInInspector]
    public MapDataCollection MapList = new MapDataCollection();
    [HideInInspector]
    public int MapNumber;
    [HideInInspector]
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
                MapInfo.Add(mapData);
            });
            IsMapCreated = true;
            isNewMap = true;
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
            return;
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

    public void SaveToJson(string name)
    {
        JsonData MapType;
        if (MapInfo.Count == 0 )
            Debug.Log("Don't have inforamtion of map");
        else
        {
            MapList.MapData = MapInfo;
            MapType = JsonMapper.ToJson(MapList);
            var fileName = Path.Combine(Application.dataPath,"Map", name+ ".json");
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

    public void ReloadMap()
    {
        if (StoredMap.Count == 0)
            return;
        else
        {
            var mapName = StoredMap[MapIndex];
            var fileName = Path.Combine(Application.dataPath, "Map", mapName);
            var MapInformation = File.ReadAllText(fileName);
            MapDataCollection MapData = JsonMapper.ToObject<MapDataCollection>(MapInformation);
            var MapDatas = MapData.MapData;
            DestroyMap();
            BuildMapGrids();
            foreach (MapTile data in MapTilesList)
            {
                for (int i = 0; i < MapDatas.Count; i++)
                {
                    if (data.X == MapDatas[i].X && data.Y == MapDatas[i].Y)
                    {
                        data.MapData.Type = MapDatas[i].Type;
                        if (data.MapData.Type != TileType.None)
                        {
                            var material = data.transform.Find("Material").GetComponent<SpriteRenderer>();
                            material.sprite = Resources.Load<Sprite>("UI/MapTile/" + data.MapData.Type.ToString());
                        }
                        data.MapData.SortingOrder = MapDatas[i].SortingOrder;
                        var Mesh = data.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                        data.MapData.RespawnForChamp = MapDatas[i].RespawnForChamp;
                        if (data.MapData.RespawnForChamp == true)
                            Mesh.color = Color.red;
                        data.MapData.RespawnForEnemy = MapDatas[i].RespawnForEnemy;
                        if (data.MapData.RespawnForEnemy == true)
                            Mesh.color = Color.blue;
                    }
                }
            }
            ShowRespawnGrid();
            isNewMap = false;
        }   
    }

}
