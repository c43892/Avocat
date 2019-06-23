using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataWindow : EditorWindow
{
   public string FileName = "";
    
    void OnGUI()
    {
        EditorGUILayout.PrefixLabel("Please enter the file name");
        FileName = EditorGUILayout.TextField(FileName);
        if (GUILayout.Button("Ok"))
        {
            if (FileName.Equals(""))
                Debug.Log("Name can't be empty");
            else
            {
                Debug.Log("Map is saved");
                GameObject.Find("Editor").GetComponent<MapCreator>().SaveToJson(FileName);
                Close();
            }
        }
    }
}
