using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Avocat;
using LitJson;
using System.IO;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MapReader))]
public class MapReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
       // base.DrawDefaultInspector();
        MapReader MapReader = (MapReader)target;

        // 读取地图信息
        EditorGUILayout.Space();
        var fileName = Path.Combine(Application.dataPath, "Map");
        MapReader.GetDirs(fileName, ref MapReader.MapInfo);
        GUIContent arrayLabel = new GUIContent("Choose the Map");
        MapReader.ArrayIndex = EditorGUILayout.Popup(arrayLabel,MapReader.ArrayIndex, MapReader.MapInfo.ToArray());

        // 回到地图编辑器
        EditorGUILayout.Space();
        if (GUILayout.Button("Go Back To MapEditor"))
        {
            var filePath = Path.Combine(Application.dataPath, "MapEditor", "MapEditor.unity");
            EditorSceneManager.OpenScene(filePath);
            Selection.activeGameObject = GameObject.Find("Editor");
        }
    }
}
