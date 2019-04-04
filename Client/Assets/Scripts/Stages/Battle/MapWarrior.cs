using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

public class MapWarrior : MonoBehaviour
{
    public BattleStage BattleStage { get; set; }
    public Vector2 CenterOffset = new Vector2(0.5f, 0.5f);

    // 对应的角色
    public Warrior Warrior { get { return BattleStage.Map.Warriors[X, Y]; } }

    public int X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    } int x;

    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    } int y;

    public SpriteRenderer SpriteRender {
        get
        {
            if (sr == null)
                sr = GetComponent<SpriteRenderer>();

            return sr;
        }
    } SpriteRenderer sr;

    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            SpriteRender.color = selected ? Color.green : Color.white;
        }
    } bool selected = false;
}
