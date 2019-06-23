using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using Avocat;
using Swift;
using System;


public class MapReader : MonoBehaviour
{
    public GameObject MapRoot;
    public List<Transform> MapTile = new List<Transform>();
    public List<MapData> Map = new List<MapData>();
    public List<MapData> RespawnForChamp = new List<MapData>();
    public List<MapData> RespawnForEnemy = new List<MapData>();
    public int ArrayIndex = 0;
    public List<string> MapInfo = new List<string>();

    // 将Json转换成地图信息
    public void ReloadMapInfo()
    {
        var fileName = Path.Combine(Application.dataPath, "Map", MapInfo[ArrayIndex]);
        var MapInformation = File.ReadAllText(fileName);
        MapDataCollection MapData = JsonMapper.ToObject<MapDataCollection>(MapInformation);
        Map = MapData.MapData;

        // 将英雄和怪物出生点储存
        FC.For(0, Map.Count, (i) =>
        {
            if(Map[i].RespawnForChamp == true)
                RespawnForChamp.Add(Map[i]);
            if (Map[i].RespawnForEnemy == true)
                RespawnForEnemy.Add(Map[i]);
        });
        
        GetRandomRespawnPlace(RespawnForChamp);
        GetRandomRespawnPlace(RespawnForEnemy);
    }

    public bool IsMapTileInMapRoot()
    {
        foreach (Transform child in MapRoot.transform)
        {
            if (child.CompareTag("MapTile"))
                MapTile.Add(child.transform);
        }
        if (MapTile.Count == 0)
            return false;
        else
            return true;
    }

    // 将MapData的数据传给tile
    public void ReadMapInfo(MapTile tile)
    {
        for (int i = 0; i < Map.Count; i++)
        {
            if (tile.X == Map[i].X && tile.Y == Map[i].Y)
            {
                tile.MapData.Type = Map[i].Type;
                var material = tile.transform.Find("Material").GetComponent<SpriteRenderer>();
                if (Map[i].Type != TileType.None)
                    material.sprite = Resources.Load<Sprite>("UI/MapTile/" + Map[i].Type.ToString());                   
                material.sortingOrder =Map[i].SortingOrder;

                if (Map[i].RespawnForChamp == true || Map[i].RespawnForEnemy == true)
                {
                    if (Map[i].RespawnForChamp == true)
                        tile.MapData.RespawnForChamp = true;
                    else
                        tile.MapData.RespawnForEnemy = true;
                    tile.transform.Find("RespawnPlace").gameObject.SetActive(true);
                    var Mesh = tile.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                    Mesh.sortingOrder = Map[i].SortingOrder+1;
                }
                return;
            }
        }
    }

    // 随机打乱出生顺序
    public void GetRandomRespawnPlace(List<MapData> RespawnList)
    {
        FC.For(RespawnList.Count, (i) =>
        {
            int RandomNum = UnityEngine.Random.Range(0, RespawnList.Count);
            var temp = RespawnList[RandomNum];
            RespawnList[RandomNum] = RespawnList[i];
            RespawnList[i] = temp;
        });
    }

    public static void GetDirs(string dirPath, ref List<string> dirs)
    {
        dirs.Clear();
        foreach (string path in Directory.GetFiles(dirPath))
        {
            // 获取所有文件夹中包含后缀为 .json 的路径
            if (System.IO.Path.GetExtension(path) == ".json")
            {
                var Path = path.Substring(path.IndexOf("Map")).Substring(4);
                dirs.Add(Path);
            }
        }
    }
}
