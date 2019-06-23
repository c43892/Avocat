using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Avocat;
using LitJson;
using UnityEditor.SceneManagement;
using System.IO;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        MapCreator MapCreator = (MapCreator)target;
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 20;
        guiStyle.alignment = TextAnchor.MiddleCenter;


        // 锁定当前的inspector
        ActiveEditorTracker.sharedTracker.isLocked = true;

        // 标题
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("MapEditor", guiStyle);
        GUILayout.EndHorizontal();

        // 选择地图背景
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("BackGround");
        MapCreator.BackGround.sprite = (Sprite)EditorGUILayout.ObjectField(MapCreator.BackGround.sprite, typeof(Sprite), allowSceneObjects: true);

        // 地图信息列表
        EditorGUILayout.Space();
        var fileName = Path.Combine(Application.dataPath, "Map");
        MapReader.GetDirs(fileName,ref MapCreator.StoredMap);
        GUIContent arrayLabel = new GUIContent("Reload the Map");
        MapCreator.MapIndex = EditorGUILayout.Popup(arrayLabel, MapCreator.MapIndex, MapCreator.StoredMap.ToArray());

        // 载入地图信息
        EditorGUILayout.Space();
        if (GUILayout.Button("Reload Map"))
        {
            MapCreator.ReloadMap();
            Selection.activeGameObject = null;
        }

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
            Selection.activeGameObject = null;
        }

        if (GUILayout.Button("Rock"))
        {
            MapCreator.TileType = TileType.Rock;
            Selection.activeGameObject = null;
        }

        if (GUILayout.Button("Soil"))
        {
            MapCreator.TileType = TileType.Soil;
            Selection.activeGameObject = null;
        }
        if (GUILayout.Button("None"))
        {
            MapCreator.TileType = TileType.None;
            Selection.activeGameObject = null;
        }

        // 显示当前选择的材质
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Current Material");
        EditorGUILayout.TextField(MapCreator.TileType.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Show RespawnGrid"))
        {
            MapCreator.ShowRespawnGrid();
            Selection.activeGameObject = null;
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Hide RespawnGrid"))
        {
            MapCreator.HideRespawnGrid();
            Selection.activeGameObject = null;
        }

        // 选择出生点类型
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Choose RespawnType");
        if (GUILayout.Button("Hero"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.Hero;
            Selection.activeGameObject = null;
        }
        if (GUILayout.Button("Enemy"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.Enemy;
            Selection.activeGameObject = null;
        }
        if (GUILayout.Button("None"))
        {
            MapCreator.respawnType = MapCreator.RespawnType.None;
            Selection.activeGameObject = null;
        }
        EditorGUILayout.PrefixLabel("Current Respawn Type");
        EditorGUILayout.TextField(MapCreator.respawnType.ToString());


        // 储存文件
        EditorGUILayout.Space();
        if (GUILayout.Button("Save Map"))
        {
           SaveDataWindow window = (SaveDataWindow)EditorWindow.GetWindow(typeof(SaveDataWindow), false, "Store the name of Map");
           window.minSize = new Vector2(200, 100);
            if (MapCreator.isNewMap == false && MapCreator.StoredMap.Count != 0)
            {
                var name = MapCreator.StoredMap[MapCreator.MapIndex];
                window.FileName = name.Substring(0,name.Length - 5);
            }    
           window.Show();
        }


        // 返回游戏界面
        EditorGUILayout.Space();
        if (GUILayout.Button("Go Back To Game"))
        {
            var filePath = Path.Combine(Application.dataPath, "Test", "CombatTestScene.unity");
            EditorSceneManager.OpenScene(filePath);
            Selection.activeGameObject = GameObject.Find("MapReader");
        }
    }
}
