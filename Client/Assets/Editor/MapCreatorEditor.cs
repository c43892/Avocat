using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

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
            MapCreator.EditMap.BuildMapGrids();
        }

        // 销毁地图选项
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Destroy Map");       
        if (GUILayout.Button("Destroy Map"))
        {
            MapCreator.EditMap.DestroyMap();
        }

        // 选择地图材质
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Material of MapTile");
        if (GUILayout.Button("Grass")) {
            MapCreator.materialType = MapCreator.MapMaterialtype.Grass;
        }

        if (GUILayout.Button("Water"))
        {
            MapCreator.materialType = MapCreator.MapMaterialtype.Water;
        }

        if (GUILayout.Button("Mountain"))
        {
            MapCreator.materialType = MapCreator.MapMaterialtype.Mountain;
        }

        // 显示当前选择的材质
        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Current Material");
        EditorGUILayout.TextField(MapCreator.materialType.ToString());
    }
}
