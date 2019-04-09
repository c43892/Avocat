using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

public class MapAvatar : MonoBehaviour
{
    public BattleStage BattleStage { get; set; }
    public Vector2 CenterOffset = new Vector2(0.5f, 0.5f);

    public TextMesh HpText;
    public TextMesh PowerText;
    public TextMesh ShieldText;

    // 对应的角色
    public Warrior Warrior { get; set; }

    public void RefreshAttrs()
    {
        HpText.text = Warrior.Hp.ToString();
        PowerText.text = Warrior.Power.ToString();
        ShieldText.text = Warrior.Shield.ToString();
        ShieldText.transform.parent.gameObject.SetActive(Warrior.Shield > 0);

        Color = Warrior.ActionDone ? ColorAttacked : ColorDefault;
    }

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

    public static readonly Color ColorDefault = Color.white;
    public static readonly Color ColorSelected = Color.green;
    public static readonly Color ColorAttacked = Color.gray;

    public Color Color
    {
        get
        {
            return sr.color;
        }
        set
        {
            sr.color = value;
        }
    }
}
