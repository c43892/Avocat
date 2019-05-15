using System;
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

    // 延迟刷新属性显示，值按照当前属性值算
    public Action DelayRefreshAttrs()
    {
        var hp = Warrior.HP;
        var atk = Warrior.BasicAttackValue;
        var es = Warrior.ES;
        var actionDone = Warrior.ActionDone;
        return () => RefreshAttrs2(hp, atk, es, actionDone);
    }

    // 刷新属性值到指定值
    public void RefreshAttrs2(int hp, int basicAttackValue, int es, bool actionDone)
    {
        HpText.text = hp.ToString();
        PowerText.text = basicAttackValue.ToString();
        ShieldText.text = es.ToString();
        ShieldText.transform.parent.gameObject.SetActive(es > 0);
        Color = actionDone ? ColorActionDone : ColorDefault;
    }

    // 刷新属性值
    public void RefreshAttrs()
    {
        NameText.text = Warrior.Name;
        RefreshAttrs2(Warrior.HP, Warrior.BasicAttackValue, Warrior.ES, Warrior.ActionDone);
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
