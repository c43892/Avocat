using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class OperationOnMap : MonoBehaviour
{
    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
        }
        SceneView.duringSceneGui += OnScene;
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;
        GameObject ob = Selection.activeGameObject;
        if (ob != null && ob.name.Equals("EditMaptile(Clone)"))
        {
            var material = ob.transform.Find("Material").GetComponent<SpriteRenderer>();
            material.sprite = Resources.Load<Sprite>("UI/MapTile/" + MapCreator.materialType.ToString());
          //  var name = material.Find("Name").GetComponent<TextMesh>();
           // name.text = MapCreator.materialType.ToString();
        }
        e.Use();
    }   
}
