using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Avocat;
using LitJson;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        //var style = new GUIStyle(GUI.skin.button);
        MapCreator MapCreator = (MapCreator)target;
        //style.normal.textColor = Color.red;
        //style.active.textColor = Color.green;        
        //EditorGUILayout.BeginVertical();

        // 选择地图背景
        EditorGUILayout.PrefixLabel("BackGround");
        MapCreator.BackGround.sprite = (Sprite)EditorGUILayout.ObjectField(MapCreator.BackGround.sprite, typeof(Sprite), allowSceneObjects: true);

        // 创建地图选项
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Create Map");
        if (GUILayout.Button("Create Map"))
        {
            MapCreator.BuildMapGrids();
        }

        // 销毁地图选项
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Destroy Map");       
        if (GUILayout.Button("Destroy Map"))
        {
            MapCreator.DestroyMap();
            MapCreator.MapInfo.Clear();
        }

        // 选择地图材质
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Material of MapTile");
        if (GUILayout.Button("Grass")) {
            MapCreator.TileType = TileType.Grass;
        }

        if (GUILayout.Button("Rock"))
        {
            MapCreator.TileType = TileType.Rock;
        }

        if (GUILayout.Button("Soil"))
        {
            MapCreator.TileType = TileType.Soil;
        }
        if (GUILayout.Button("None"))
        {
            MapCreator.TileType = TileType.None;
        }

        // 显示当前选择的材质
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Current Material");
        EditorGUILayout.TextField(MapCreator.TileType.ToString());

        EditorGUILayout.Space();
        //EditorGUILayout.PrefixLabel("Create RespawnGrid");
        if (GUILayout.Button("Create RespawnGrid"))
        {
            MapCreator.ShowRespawnGrid();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Destroy RespawnGrid"))
        {
            MapCreator.HideRespawnGrid();
        }


        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Choose RespawnType");
        if (GUILayout.Button("Hero"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.Hero;
        }
        if (GUILayout.Button("Enemy"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.Enemy;
        }
        if (GUILayout.Button("None"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.None;
        }
        EditorGUILayout.PrefixLabel("Current Respawn Type");
        EditorGUILayout.TextField(MapCreator.respawnType.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Save Map"))
        {
            MapCreator.SaveToJson();
        }

        
    }
}
