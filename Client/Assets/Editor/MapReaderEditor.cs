using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using Avocat;
using LitJson;


public class MapReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        MapReader MapReader = (MapReader)target;
        if (GUILayout.Button("Reload MapInfo"))
        {
          //  MapReader.ReadMapInfo();
        }
    }
}
