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

    // 储存地图信息
    public List<MapData> Map = new List<MapData>();

    // 储存英雄出生点
    public List<MapData> RespawnForChamp = new List<MapData>();

    // 储存敌人出生点
    public List<MapData> RespawnForEnemy = new List<MapData>();

    // 选取地图文件时的index
    public int ArrayIndex = 0;

    // 储存所有地图文件名
    public List<string> MapNameList = new List<string>();

    // 将Json转换成地图信息
    public List<MapData> ReloadMapInfo()
    {
        var fileName = Path.Combine(Application.dataPath, "Map", MapNameList[ArrayIndex]);
        var MapInformation = File.ReadAllText(fileName);
        MapDataCollection MapData = JsonMapper.ToObject<MapDataCollection>(MapInformation);
        Map = MapData.MapData;
        return Map;
    }

    // 根据名字加载地图
    public void ReloadMapByName(string name)
    {
        var fileName = Path.Combine(Application.dataPath, "Map", name);
        var MapInformation = File.ReadAllText(fileName);
        MapDataCollection MapData = JsonMapper.ToObject<MapDataCollection>(MapInformation);
        Map = MapData.MapData;
    }

    // 寻找所有Map文件
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

    // 找出最近保存的文件index
    public void FindCurrentMapIndex()
    {
        for (int i = 0; i < MapNameList.Count; i++)
        {
            if (MapNameList[i].Equals(MapCreator.currentFileName + ".json"))
            {
                ArrayIndex = i;
                return;
            }
        }
    }
}
