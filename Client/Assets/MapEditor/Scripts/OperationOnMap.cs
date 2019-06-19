using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Avocat;

[ExecuteInEditMode]
public class OperationOnMap : MonoBehaviour
{
    public MapCreator MapCreator;
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnScene;
    }

    void OnScene(SceneView scene)
    {

        // Event e = Event.current;
        GameObject ob = Selection.activeGameObject;
        if (ob != null && ob.name.Equals("EditMaptile(Clone)"))
        {
            var material = ob.transform.Find("Material").GetComponent<SpriteRenderer>();
            if (MapCreator.TileType != TileType.None)
            {
                material.sprite = Resources.Load<Sprite>("UI/MapTile/" + MapCreator.TileType.ToString());
                var mapTile = ob.GetComponent<MapTile>();
                if (MapCreator.MapInfo.Count != 0)
                {
                    
                    // var Data = MapCreator.MapInfo.Find(data => (data == mapTile.MapData));
                   // var Data = MapCreator.MapInfo.Find(data => (data.X == mapTile.X) && (data.Y == mapTile.Y));
                   // var Data = MapCreator.MapInfo.Find(data => (data.X == ob.transform.localPosition.x) && (data.Y == ob.transform.localPosition.y));
                  //  Data.Type = MapCreator.TileType;
                    mapTile.MapData.Type = MapCreator.TileType;
                }
                if (ob.transform.Find("RespawnPlace").gameObject.activeSelf)
                {
                    var Mesh = ob.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                    if (MapCreator.respawnType == MapCreator.RespawnType.Hero)
                    {
                        Mesh.color = Color.red;
                        mapTile.MapData.RespawnForChamp = true;
                        mapTile.MapData.RespawnForEnemy = false;
                    }

                    else if (MapCreator.respawnType == MapCreator.RespawnType.Enemy)
                    {
                        Mesh.color = Color.blue;
                        mapTile.MapData.RespawnForEnemy = true;
                        mapTile.MapData.RespawnForChamp = false;
                    }
                    else
                    {
                        Mesh.color = new Color32(255,255,255,86);
                        mapTile.MapData.RespawnForChamp = false;
                        mapTile.MapData.RespawnForEnemy = false;
                    }   
                }

            }
            else
                material.sprite = null;
        }
        // e.Use();
    }
}
