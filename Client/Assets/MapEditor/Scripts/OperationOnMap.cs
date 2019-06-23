using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Avocat;

[ExecuteInEditMode]
public class OperationOnMap : MonoBehaviour
{
    public MapCreator MapCreator;
    public GameObject MapRoot;

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnScene;
    }

    void OnScene(SceneView scene)
    {
        GameObject ob = Selection.activeGameObject;
        Event e = Event.current;
        
        if (e.button == 1 && e.isMouse)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePosition = mouseRay.GetPoint(0f);
            WorldPos2MapPos(mousePosition.x, mousePosition.y, out float gx, out float gy);
            var tile = MapCreator.MapTilesList.Find(t => t.X == (int)gx && t.Y == (int)gy);
            if (tile != null)
            {
                var respawn = tile.transform.Find("RespawnPlace").gameObject;
                var mat = tile.transform.Find("Material").GetComponent<SpriteRenderer>();
                var Mesh = tile.transform.Find("RespawnPlace").GetComponent<SpriteRenderer>();
                if (!respawn.activeSelf)
                {
                    mat.sprite = null;
                    tile.MapData.Type = TileType.None;
                    Selection.activeGameObject = null;
                }
                else
                {
                    Mesh.color = new Color32(255, 255, 255, 86);
                    tile.MapData.RespawnForChamp = false;
                    tile.MapData.RespawnForEnemy = false;
                    Selection.activeGameObject = null;
                }
            }  
        }

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

    public void WorldPos2MapPos(float x, float y, out float tx, out float ty)
    {
        var p = MapRoot.transform.worldToLocalMatrix.MultiplyPoint(new Vector2(x, y));
        tx = p.x;
        ty = p.y;
    }
}
