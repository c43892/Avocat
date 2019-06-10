using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public SpriteRenderer BackGround;
    public EditMap EditMap;
    public  Sprite Sprite{
        get { return BackGround.sprite; }
        set { BackGround.sprite = value; }
    }
    public enum MapMaterialtype
    {
        Grass,
        Rock,
        Soil,
    }
    public static MapMaterialtype materialType;
}
