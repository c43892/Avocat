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
    public BattleMap Map { get; set; }
    public MapTile MapTile;
    public Transform MapRoot;

    // 储存地块
    public List<MapTile> MapTilesList = new List<MapTile>();

    // 判断当前Map是否存在
    public bool IsMapCreated { get; set; }

    // Map保存的文件名信息
    public List<string> StoredMap = new List<string>();

    // 用于加载Map的index
    public int MapIndex;

    // 判断是否为新的地图
    public bool isNewMap;

    // 用于储存保存后的文件名
    public static string currentFileName = "";

    // 储存地图信息
    public List<MapData> MapInfo = new List<MapData>();
    public MapDataCollection MapList = new MapDataCollection();

    // 地块类型
    public TileType TileType;

    // 出生点类型
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
  
    // 构建新的地块层
    public void BuildMapGrids()
    {
        JudgeMapStatus();
        if (!IsMapCreated)
        {
            Map = new BattleMap(10, 6, (x, y) => TileType.None);
            FC.For2(Map.Width, Map.Height, (x, y) =>
            {
                var tile = Instantiate(MapTile);
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TestRes/BattleMap/MapTile");
                tile.transform.SetParent(MapRoot);
                tile.X = x;
                tile.Y = y;
                var material = tile.transform.Find("Material").GetComponent<SpriteRenderer>();
                var respawnPlace = tile.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                material.sortingOrder = y + 2;
                respawnPlace.sortingOrder = y + 3;
                tile.gameObject.SetActive(true);
                MapTilesList.Add(tile);

                // MapData没有new之前就传递是值传递
                tile.GetComponent<MapTile>().MapData = new MapData(x, y, TileType.None, material.sortingOrder, respawnPlace.sortingOrder);
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

    // 摧毁当前Map
    public void DestroyMap()
    {
        JudgeMapStatus();
        if (IsMapCreated)
        {
            foreach (MapTile map in MapTilesList)
                DestroyImmediate(map.gameObject);
            MapTilesList.Clear();
            MapInfo.Clear();
            IsMapCreated = false;
        }
        else
            return;
    }

    // 判断当前是否有地图存在
    public void JudgeMapStatus()
    {
        if (MapTilesList != null && MapTilesList.Count > 0)
            IsMapCreated = true;
        else
            IsMapCreated = false;
    }

    // 将地图文件存为Json
    public void SaveToJson(string name)
    {       
        if (MapInfo.Count == 0 )
            Debug.Log("Don't have inforamtion of map");
        else
        {
            JsonData MapType;

            // 记录文件名
            currentFileName = name;
            MapList.MapData = MapInfo;
            MapType = JsonMapper.ToJson(MapList);
            var fileName = Path.Combine(Application.dataPath,"Map", name+ ".json");

            // 保存文件
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, MapType.ToString());
            Debug.Log("Map information is saved!");
        }  
    }

    // 显示出生点地块
    public void ShowRespawnGrid()
    {
        var Maptile = GameObject.FindGameObjectsWithTag("MapTile");
        FC.For(Maptile.Length, (i) =>
        {
            Maptile[i].transform.Find("RespawnPlace").gameObject.SetActive(true);
        });
    }

    // 隐藏出生点地块
    public void HideRespawnGrid()
    {
        var Maptile = GameObject.FindGameObjectsWithTag("MapTile");
        FC.For(Maptile.Length, (i) =>
        {
            Maptile[i].transform.Find("RespawnPlace").gameObject.SetActive(false);
        });
    }

    // 加载地图
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

            // 加载前先清空地图信息
            DestroyMap();
            BuildMapGrids();

            // 将Map信息载入地块
            var MapDatas = MapData.MapData;
            foreach (MapTile data in MapTilesList)
            {
                for (int i = 0; i < MapDatas.Count; i++)
                {
                    if (data.X == MapDatas[i].X && data.Y == MapDatas[i].Y)
                    {
                        // 加载地块信息
                        var mapData = data.MapData;
                        mapData.Type = MapDatas[i].Type;
                        if (mapData.Type != TileType.None)
                        {
                            var material = data.transform.Find("Material").GetComponent<SpriteRenderer>();
                            material.sprite = Resources.Load<Sprite>("UI/MapTile/" + mapData.Type.ToString());
                        }
                        mapData.MaterialSortingOrder = MapDatas[i].MaterialSortingOrder;
                        mapData.RespawnSortingOrder = MapDatas[i].RespawnSortingOrder;

                        // 加载出生点信息
                        var Mesh = data.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                        mapData.RespawnForChamp = MapDatas[i].RespawnForChamp;
                        if (mapData.RespawnForChamp == true)
                            Mesh.color = Color.red;
                        mapData.RespawnForEnemy = MapDatas[i].RespawnForEnemy;
                        if (mapData.RespawnForEnemy == true)
                            Mesh.color = Color.blue;
                    }
                }
            }
            ShowRespawnGrid();
            isNewMap = false;
        }   
    }
}
