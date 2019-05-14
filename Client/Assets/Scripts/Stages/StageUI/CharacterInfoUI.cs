using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class CharacterInfoUI : MonoBehaviour
{
    public GameObject ChampPhoto;
    public GameObject ChampType;
    public GameObject ChampName;
    public GameObject AttackValue;
    public GameObject MagicValue;
    public GameObject ArmorValue;
    public GameObject MagicResistValue;
    public GameObject ShieldBar;
    public GameObject ShieldValue;
    public GameObject LifeBar;
    public GameObject LifeValue;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowChampPhoto(Warrior warrior)
    {
        ChampPhoto.GetComponent<Image>().sprite = Resources.Load("UI/ChampPhoto/" + warrior.EnglishName, typeof(Sprite)) as Sprite;
    }

    public void ShowChampType(Warrior warrior)
    {
        ChampType.GetComponent<Image>().sprite = Resources.Load("UI/CardType/" + (warrior as Hero).CardType, typeof(Sprite)) as Sprite;
    }

    public void ShowAttackValue(Warrior warrior)
    {
        AttackValue.GetComponent<Text>().text = warrior.ATK.ToString();
    }
    public void ShowArmorValue(Warrior warrior)
    {
        ArmorValue.GetComponent<Text>().text = warrior.POW.ToString();
    }
    public void ShowMagicValue(Warrior warrior)
    {
        MagicValue.GetComponent<Text>().text = warrior.ARM.ToString();
    }
    public void ShowRESValue(Warrior warrior)
    {
        MagicResistValue.GetComponent<Text>().text = warrior.RES.ToString();
    }
    public void Showshield(Warrior warrior)
    {
        ShieldValue.GetComponent<Text>().text = warrior.ES.ToString() + "/" + warrior.MaxES.ToString();
    }
    public void ShowLife(Warrior warrior)
    {
        LifeValue.GetComponent<Text>().text = warrior.HP.ToString() + "/" + warrior.MaxHP.ToString();
    }

    public void UpdateWarriorInfo(Warrior warrior)
    {
        ShowChampPhoto(warrior);
        // ShowChampType(warrior);
        ShowChampName(warrior);
        ShowAttackValue(warrior);
        ShowArmorValue(warrior);
        ShowMagicValue(warrior);
        ShowRESValue(warrior);
        UpDateShield(warrior,0);
        UpDateLife(warrior,0);
    }

    public void UpDateLife(Warrior warrior, int value)
    {
        LifeValue.GetComponent<Text>().text = warrior.HP.ToString() + "/" + warrior.MaxHP.ToString();
        LifeBar.GetComponent<RectTransform>().anchorMax = new Vector2(warrior.HP / (float)warrior.MaxHP, 1);
    }

    public void UpDateShield(Warrior warrior, int value)
    {
        ShieldValue.GetComponent<Text>().text = warrior.ES.ToString() + "/" + warrior.MaxES.ToString();
        ShieldBar.GetComponent<RectTransform>().anchorMax = new Vector2(warrior.ES / (float)warrior.MaxES, 1);
    }

    public void ShowChampName(Warrior warrior)
    {
        ChampName.GetComponent<Text>().text = warrior.Name;
    }
}

