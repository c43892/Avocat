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
    public List<Transform> children = new List<Transform>();
    public List<MapData> Map = new List<MapData>();
    public List<MapData> RespawnForChamp = new List<MapData>();
    public List<MapData> RespawnForEnemy = new List<MapData>();
    bool[] OccupiedForChamp;
    int NumOfChampRespawn;
    bool[] OccupiedForEnemy;
    int NumOfEnemyRespawn;
    public Action<int, int,Warrior>  onSetWarrior = null;

    public void ReloadMapInfo()
    {
        var fileName = Path.Combine(Application.dataPath, "Map", "MapInfo.json");
        var MapInfo = File.ReadAllText(fileName);
        MapDataCollection MapData = JsonMapper.ToObject<MapDataCollection>(MapInfo);
        Map = MapData.MapData;
        GetRespawnForChamp();
        GetRespawnForEnemy();
    }

    public void FindMapTileInMapRoot()
    {
        foreach (Transform child in MapRoot.transform)
        {
            if (child.CompareTag("MapTile"))
                children.Add(child.transform);
        }
    }

    public void ReadMapInfo(MapTile tile)
    {
        for (int i = 0; i < Map.Count; i++)
        {
            if (tile.X == Map[i].X && tile.Y == Map[i].Y)
            {
                var material = tile.transform.Find("Material").GetComponent<SpriteRenderer>();
                if (Map[i].Type != TileType.None)
                {
                    material.sprite = Resources.Load<Sprite>("UI/MapTile/" + Map[i].Type.ToString());
                    tile.MapData.Type = Map[i].Type;
                }
                    
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

    public void GetRespawnForChamp()
    {
        if (Map.Count == 0)
            return;
        FC.For(Map.Count, (i) =>
        {
            if (Map[i].RespawnForChamp == true)
                RespawnForChamp.Add(Map[i]);
        });
        OccupiedForChamp = new bool[RespawnForChamp.Count];
        NumOfChampRespawn = RespawnForChamp.Count;
    }

    public void GetRespawnForEnemy()
    {
        if (Map.Count == 0)
            return;
        FC.For(Map.Count, (i) =>
        {
            if (Map[i].RespawnForEnemy == true)
                RespawnForEnemy.Add(Map[i]);
        });
        OccupiedForEnemy = new bool[RespawnForEnemy.Count];
        NumOfEnemyRespawn = RespawnForEnemy.Count;
    }

    public MapData GetRandomPlaceForChamp()
    {
        int num = -1;
        var MaxRange = RespawnForChamp.Count;
        bool FindNumber = false;
        while (!FindNumber || NumOfChampRespawn == 0)
        {
            num = UnityEngine.Random.Range(0, MaxRange);
            if (!OccupiedForChamp[num])
            {
                OccupiedForChamp[num] = true;
                NumOfChampRespawn--;
                FindNumber = true;
            }
        }
        Debug.Assert(num != -1, "The number of champions are more than respawn places");
        return RespawnForChamp[num];
    }

    public MapData GetRandomPlaceForEnemy()
    {
        int num = -1;
        var MaxRange = RespawnForEnemy.Count;
        bool FindNumber = false;
        while (!FindNumber)
        {
            num = UnityEngine.Random.Range(0, MaxRange);
            if (!OccupiedForEnemy[num])
            {
                OccupiedForEnemy[num] = true;
                NumOfEnemyRespawn--;
                FindNumber = true;
            }
        }
        Debug.Assert(num != -1, "The number of enemies are more than respawn places");
        return RespawnForEnemy[num];
    }
}
