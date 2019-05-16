using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class ObstacleUI : MonoBehaviour
{
    public GameObject ObstaclePhoto;
    public Text ObstacleName;

    public void UpdateObstacleInfo(BattleMapItem item) {
        gameObject.SetActive(true);
        ObstaclePhoto.GetComponent<Image>().sprite = Resources.Load("UI/Obstacle/" + item.EnglishName, typeof(Sprite)) as Sprite;
        ObstacleName.text = item.Name;
    }
}
