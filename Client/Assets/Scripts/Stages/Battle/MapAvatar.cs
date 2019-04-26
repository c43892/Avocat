using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;

public class MapAvatar : MonoBehaviour
{
    public BattleStage BattleStage { get; set; }
    public Vector2 CenterOffset = new Vector2(0.5f, 0.5f);

    public TextMesh NameText;
    public TextMesh HpText;
    public TextMesh PowerText;
    public TextMesh ShieldText;

    // 对应的角色
    public Warrior Warrior { get; set; }

    public void RefreshAttrs()
    {
        NameText.text = Warrior.Name;
        HpText.text = Warrior.HP.ToString();
        PowerText.text = Warrior.BasicAttackValue.ToString();
        ShieldText.text = Warrior.ES.ToString();
        ShieldText.transform.parent.gameObject.SetActive(Warrior.ES > 0);

        Color = Warrior.ActionDone ? ColorActionDone : ColorDefault;
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

    public static readonly Color ColorDefault = Color.black;
    public static readonly Color ColorSelected = Color.blue;
    public static readonly Color ColorActionDone = Color.gray;

    public Color Color
    {
        get
        {
            return transform.Find("Name").Find("Name").GetComponent<TextMesh>().color;
        }
        set
        {
            transform.Find("Name").Find("Name").GetComponent<TextMesh>().color = value;
        }
    }
}
