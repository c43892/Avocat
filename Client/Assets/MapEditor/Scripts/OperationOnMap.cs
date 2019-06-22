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
        GameObject ob = Selection.activeGameObject;
        if (ob != null && ob.name.Equals("EditMaptile(Clone)"))
        {
            var respawnPlace = ob.transform.Find("RespawnPlace").gameObject;
            var mapTile = ob.GetComponent<MapTile>();

            // 编辑地图材质地块
            if (!respawnPlace.activeSelf)
            {
                var material = ob.transform.Find("Material").GetComponent<SpriteRenderer>();
                if (MapCreator.TileType != TileType.None)
                    material.sprite = Resources.Load<Sprite>("UI/MapTile/" + MapCreator.TileType.ToString());
                else
                    material.sprite = null;
                mapTile.MapData.Type = MapCreator.TileType;
            }
            else // 编辑出生点
            {
                var Mesh = ob.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();

                // 英雄出生点标记为红色
                if (MapCreator.respawnType == MapCreator.RespawnType.Hero) 
                {
                    Mesh.color = Color.red;
                    mapTile.MapData.RespawnForChamp = true;
                    mapTile.MapData.RespawnForEnemy = false;
                }
                else if (MapCreator.respawnType == MapCreator.RespawnType.Enemy) // 敌人出生点标记为蓝色
                {
                    Mesh.color = Color.blue;
                    mapTile.MapData.RespawnForEnemy = true;
                    mapTile.MapData.RespawnForChamp = false;
                }
                else //不是出生点恢复原色
                {
                    Mesh.color = new Color32(255, 255, 255, 86);
                    mapTile.MapData.RespawnForChamp = false;
                    mapTile.MapData.RespawnForEnemy = false;
                }
            }
        }
        // e.Use();
    }
}
