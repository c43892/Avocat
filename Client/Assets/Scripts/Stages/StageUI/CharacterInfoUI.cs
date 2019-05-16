using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class CharacterInfoUI : MonoBehaviour
{
    public GameObject ChampPhoto;
    public GameObject EnemyPhoto;
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
    public GameObject PassiveSkill;
    public GameObject ActiveSkill;
    public GameObject BUFFInFo;
    public GameObject[] BUFFPic;

    public void ShowWarriorPhoto(Warrior warrior)
    {
        ChampPhoto.SetActive(false);
        EnemyPhoto.SetActive(false);
        if (warrior is Hero)
        {
            ChampPhoto.SetActive(true);
            ChampPhoto.GetComponent<Image>().sprite = Resources.Load("UI/ChampPhoto/" + warrior.EnglishName, typeof(Sprite)) as Sprite;
        }
        else {
            EnemyPhoto.SetActive(true);
            EnemyPhoto.GetComponent<Image>().sprite = Resources.Load("UI/ChampPhoto/" + warrior.EnglishName, typeof(Sprite)) as Sprite;
        }
    }

    public void ShowWarriorType(Warrior warrior)
    {
        if (warrior is Hero)
        {
            ChampType.SetActive(true);
            ChampType.GetComponent<Image>().sprite = Resources.Load("UI/CardType/" + (warrior as Hero).CardType, typeof(Sprite)) as Sprite;
        }
        else {
            ChampType.SetActive(false);
        }
        
    }

    public void ShowAttackValue(Warrior warrior)
    {
        AttackValue.GetComponent<Text>().text = warrior.ATK.ToString();
    }
    public void ShowArmorValue(Warrior warrior)
    {
        ArmorValue.GetComponent<Text>().text = warrior.ARM.ToString();
    }
    public void ShowMagicValue(Warrior warrior)
    {
        MagicValue.GetComponent<Text>().text = warrior.POW.ToString();
    }
    public void ShowRESValue(Warrior warrior)
    {
        MagicResistValue.GetComponent<Text>().text = warrior.RES.ToString();
    }

    public void UpdateWarriorInfo(Warrior warrior)
    {
        gameObject.SetActive(true);
        ShowWarriorPhoto(warrior);
        UpdateSkillPicture(warrior);
        UpDateBUFF(warrior);
        ShowWarriorType(warrior);
        ShowWarriorName(warrior);
        ShowAttackValue(warrior);
        ShowArmorValue(warrior);
        ShowMagicValue(warrior);
        ShowRESValue(warrior);
        UpDateShield(warrior);
        UpDateLife(warrior);
    }

    public void UpDateLife(Warrior warrior)
    {
        LifeValue.GetComponent<Text>().text = warrior.HP.ToString() + "/" + warrior.MaxHP.ToString();
        LifeBar.GetComponent<RectTransform>().anchorMax = new Vector2(warrior.HP / (float)warrior.MaxHP, 1);
    }

    public void UpDateShield(Warrior warrior)
    {
        ShieldValue.GetComponent<Text>().text = warrior.ES.ToString() + "/" + warrior.MaxES.ToString();
        ShieldBar.GetComponent<RectTransform>().anchorMax = new Vector2(warrior.ES / (float)warrior.MaxES, 1);
    }

    public void ShowWarriorName(Warrior warrior)
    {
        ChampName.GetComponent<Text>().text = warrior.Name;
    }

    public void UpdateSkillPicture(Warrior warrior)
    {
        PassiveSkill.SetActive(false);
        ActiveSkill.SetActive(false);
        var AllPassiveSkills = (warrior as BattleMapItem).Buffs;
        if (warrior.GetDefaultActiveSkill() != null) {
            ActiveSkill.GetComponent<Image>().sprite = Resources.Load("UI/Skill/" + warrior.GetDefaultActiveSkill().Name, typeof(Sprite)) as Sprite;
            ActiveSkill.SetActive(true);
        }
        FC.For(AllPassiveSkills.Count, (i) =>
        {
            if (AllPassiveSkills[i] is PatternSkill)
            {
                ActiveSkill.GetComponent<Image>().sprite = Resources.Load("UI/Skill/" + AllPassiveSkills[i].Name, typeof(Sprite)) as Sprite;
                ActiveSkill.SetActive(true);
            }
            else if (AllPassiveSkills[i] is PassiveSkill) {
                PassiveSkill.GetComponent<Image>().sprite = Resources.Load("UI/Skill/" + AllPassiveSkills[i].Name, typeof(Sprite)) as Sprite;
                PassiveSkill.SetActive(true);
            }
        });
    }

    public void UpDateBUFF(Warrior warrior)
    {
        bool[] isBUFFPicTaken = new bool[BUFFPic.Length];
        FC.For(BUFFPic.Length, (w) =>
        {
            BUFFPic[w].SetActive(false);
        });
        FC.For(warrior.Buffs.Count, (i) =>
        {
            if (warrior.Buffs[i].isBattleBUFF)
            {
                for (int j = 0; j < BUFFPic.Length; j++)
                {
                    if (!isBUFFPicTaken[j])
                    {
                        BUFFPic[j].SetActive(true);
                        BUFFPic[j].GetComponent<Image>().sprite = Resources.Load("UI/BUFFs/" + warrior.Buffs[i].Name, typeof(Sprite)) as Sprite;                      
                        isBUFFPicTaken[j] = true;
                        break;
                    }
                }
            }
        });
    }
}

