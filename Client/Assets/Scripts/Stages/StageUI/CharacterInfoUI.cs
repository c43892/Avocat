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
    public GameObject SkillPanel;
    public GameObject ActiveSkillPanel;
    public GameObject ActiveSkillTrigger;
    public GameObject PassiveSkillPanel;
    public GameObject PassiveSkillTrigger;
    public GameObject[] PatternSkillTriggerPics;
    public Text ActiveSkillDescription;
    public Text PassiveSkillDescription;

    // 判断技能是否为显示buff
    public BuffToDisplay BuffDisplayList
    {
        get
        {
            if(buffDisplayList == null)
                buffDisplayList = Configuration.Config(new BuffToDisplay());
            return buffDisplayList;
        }
    } BuffToDisplay buffDisplayList;

    public void ShowWarriorPhoto(Warrior warrior)
    {
        ChampPhoto.SetActive(false);
        EnemyPhoto.SetActive(false);
        if (warrior is Hero)
        {
            ChampPhoto.SetActive(true);
            ChampPhoto.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ChampPhoto/" + warrior.Name) as Sprite;
        }
        else {
            EnemyPhoto.SetActive(true);
            EnemyPhoto.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ChampPhoto/" + warrior.Name) as Sprite;
        }
    }

    public void ShowWarriorType(Warrior warrior)
    {
        if (warrior is Hero)
        {
            ChampType.SetActive(true);
            ChampType.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CardType/" + (warrior as Hero).CardType) as Sprite;
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
        UpdateSkillPanelInfo(warrior);
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
        ChampName.GetComponent<Text>().text = warrior.DisplayName;
    }

    public void UpdateSkillPicture(Warrior warrior)
    {
        var PassiveSkillFrame = PassiveSkill.transform.parent.gameObject;
        var ActiveSkillFrame = ActiveSkill.transform.parent.gameObject;
        PassiveSkillFrame.SetActive(false);
        ActiveSkillFrame.SetActive(false);
        var AllPassiveSkills = warrior.Buffs;
        if (warrior.GetDefaultActiveSkill() != null) {
            ActiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.GetDefaultActiveSkill().Name) as Sprite;
            ActiveSkillFrame.SetActive(true);
        }
        FC.For(AllPassiveSkills.Length, (i) =>
        {
            if (AllPassiveSkills[i] is PatternSkill)
            {
                ActiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + AllPassiveSkills[i].Name) as Sprite;
                ActiveSkillFrame.SetActive(true);
            }
            else if (AllPassiveSkills[i] is PassiveSkill) {
                PassiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + AllPassiveSkills[i].Name) as Sprite;
                PassiveSkillFrame.SetActive(true);
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
        FC.For(warrior.Buffs.Length, (i) =>
        {
            if (BuffDisplayList.BuffsToDisplay.Contains((name)=> { return name.Equals(warrior.Buffs[i].Name); }) )
            {
                for (int j = 0; j < BUFFPic.Length; j++)
                {
                    if (!isBUFFPicTaken[j])
                    {
                        BUFFPic[j].SetActive(true);
                        BUFFPic[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/BUFFs/" + warrior.Buffs[i].Name) as Sprite;                      
                        isBUFFPicTaken[j] = true;
                        break;
                    }
                }
            }
        });
    }

    public void UpdateSkillPanelInfo(Warrior warrior)
    {
        if (warrior is Hero)
        {
            var activeSkill = warrior.GetDefaultActiveSkill();
            var activeSkillName = ActiveSkillPanel.transform.Find("SkillName").gameObject.GetComponent<Text>();
            var activeSkillIcon = ActiveSkillPanel.transform.Find("SkillIcon").gameObject.GetComponent<Image>();
            var passiveSkillName = PassiveSkillPanel.transform.Find("SkillName").gameObject.GetComponent<Text>();
            var passiveSkillIcon = PassiveSkillPanel.transform.Find("SkillIcon").gameObject.GetComponent<Image>();
            var energyTrigger = ActiveSkillTrigger.transform.Find("EnergyTrigger").gameObject.GetComponent<Text>();
            var patternTrigger = ActiveSkillTrigger.transform.Find("PatternTrigger").gameObject;

            if (warrior.GetDefaultActiveSkill() != null) // 如果是activeskill
            {
                patternTrigger.SetActive(false);
                energyTrigger.gameObject.SetActive(true);
                activeSkillName.text = activeSkill.DisplayName;
                energyTrigger.text = activeSkill.EnergyCost.ToString() + " 能量";
                activeSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.GetDefaultActiveSkill().Name) as Sprite;
                ActiveSkillDescription.text = activeSkill.SkillDescription;
            }
            else // 如果是patternskill
            {
                var AllPassiveSkills = warrior.Buffs;               
                FC.For(AllPassiveSkills.Length, (i) =>
                {
                    if (AllPassiveSkills[i] is PatternSkill)
                    {
                        var patternSkill = AllPassiveSkills[i];
                        patternTrigger.SetActive(true);
                        energyTrigger.gameObject.SetActive(false);
                        activeSkillName.text = patternSkill.DisplayName;
                        activeSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + AllPassiveSkills[i].Name) as Sprite;
                        ActiveSkillDescription.text = patternSkill.SkillDescription;

                        FC.For(PatternSkillTriggerPics.Length, (j) =>
                        {
                            PatternSkillTriggerPics[j].SetActive(false);
                        });

                        // 将SkillPanel提示栏的图片更换为pattern技能的出招顺序
                        FC.For(warrior.PatternSkill.CardsPattern.Length, (j) =>
                        {
                            PatternSkillTriggerPics[j].SetActive(true);
                            PatternSkillTriggerPics[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CardType/" + warrior.PatternSkill.CardsPattern[j]) as Sprite;
                        });
                    }
                });
            }

            // 设置被动技能的信息栏
            var Buffs = warrior.Buffs;
            FC.For(Buffs.Length, (i) =>
            {
                if (Buffs[i] is PassiveSkill)
                {
                    passiveSkillName.text = Buffs[i].DisplayName;
                    passiveSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + Buffs[i].Name) as Sprite;
                    PassiveSkillDescription.text = Buffs[i].SkillDescription;
                }
            });
        }
    }

    public void HideSkillPanel()
    {
        SkillPanel.SetActive(false);
    }

    public void ShowSkillPanel()
    {
        SkillPanel.SetActive(true);
    }
}

