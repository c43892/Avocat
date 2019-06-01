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
        Water,
        Mountain,
    }
    public static MapMaterialtype materialType;
}
