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

    public void UpdateObstacleInfo(BattleMapObj item) {
        gameObject.SetActive(true);
        ObstaclePhoto.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Obstacle/" + item.Name) as Sprite;
        ObstacleName.text = item.DisplayName;
    }
}
