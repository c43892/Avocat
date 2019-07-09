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
    private BuffToDisplay buffDisplayList;
    public BuffToDisplay BuffDisplayList
    {
        get
        {
            return buffDisplayList;
        }
        set
        {
            buffDisplayList = value;
        }
    }

    // 显示人物信息栏头像
    public void ShowWarriorPhoto(Warrior warrior)
    {
        ChampPhoto.SetActive(false);
        EnemyPhoto.SetActive(false);
        if (warrior is Hero)
        {
            ChampPhoto.SetActive(true);
            ChampPhoto.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ChampPhoto/" + warrior.ID) as Sprite;
        }
        else {
            EnemyPhoto.SetActive(true);
            EnemyPhoto.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ChampPhoto/" + warrior.ID) as Sprite;
        }
    }

    // 显示人物类型
    public void ShowWarriorType(Warrior warrior)
    {
        ChampType.SetActive(false);
        if (warrior is Hero)
        {
            var cardType = (warrior as Hero).CardType;
            if (cardType != null)
            {
                ChampType.SetActive(true);
                ChampType.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CardType/" + cardType) as Sprite;
            }
        }
    }

    public void ShowAttackValue(int atk)
    {
        AttackValue.GetComponent<Text>().text = atk.ToString();
    }
    public void ShowArmorValue(int arm)
    {
        ArmorValue.GetComponent<Text>().text = arm.ToString();
    }
    public void ShowMagicValue(int pow)
    {
        MagicValue.GetComponent<Text>().text = pow.ToString();
    }
    public void ShowRESValue(int res)
    {
        MagicResistValue.GetComponent<Text>().text = res.ToString();
    }

    public void UpDateLife(int hp, int maxHP)
    {
       LifeValue.GetComponent<Text>().text = hp.ToString() + "/" + maxHP.ToString();
       LifeBar.GetComponent<RectTransform>().anchorMax = new Vector2( hp / (float)maxHP, 1); 
    }

    public void UpDateShield(int es, int maxES)
    {
        ShieldValue.GetComponent<Text>().text = es.ToString() + "/" + maxES.ToString();
        ShieldBar.GetComponent<RectTransform>().anchorMax = new Vector2(es / (float)maxES, 1);
    }


    public void UpdateWarriorInfo(Warrior warrior, Dictionary<string, object> warriorInfo = null)
    {
        gameObject.SetActive(true);
        ShowWarriorPhoto(warrior);
        UpdateSkillPicture(warrior);
        UpDateBUFF(warrior);
        ShowWarriorType(warrior);
        ShowWarriorName(warrior);
        UpdateSkillPanelInfo(warrior);

        // 一般用于点击人物时刷新
        if (warriorInfo == null)
        {
            ShowAttackValue(warrior.ATK);
            ShowArmorValue(warrior.ARM);
            ShowMagicValue(warrior.POW);
            ShowRESValue(warrior.RES);
            UpDateShield(warrior.ES, warrior.MaxES);
            UpDateLife(warrior.HP, warrior.MaxHP);
        }
        else // 一般用于动画播放时刷新
        {
            var atk = (int)warriorInfo["AttackValue"];
            var arm = (int)warriorInfo["ArmorValue"];
            var pow = (int)warriorInfo["MagicValue"];
            var res = (int)warriorInfo["RESValue"];
            var es = (int)warriorInfo["ShieldValue"];
            var maxES = (int)warriorInfo["MaxShieldValue"];
            var hp = (int)warriorInfo["HPValue"];
            var maxHP = (int)warriorInfo["MaxHPValue"];
            ShowAttackValue(atk);
            ShowArmorValue(arm);
            ShowMagicValue(pow);
            ShowRESValue(res);
            UpDateShield(es, maxES);
            UpDateLife(hp, maxHP);
        }
    }

   

    public void ShowWarriorName(Warrior warrior)
    {
        ChampName.GetComponent<Text>().text = warrior.DisPlayName();
    }

    // 更新血条旁边的技能图标
    public void UpdateSkillPicture(Warrior warrior)
    {
        var PassiveSkillFrame = PassiveSkill.transform.parent.gameObject;
        var ActiveSkillFrame = ActiveSkill.transform.parent.gameObject;
        PassiveSkillFrame.SetActive(false);
        ActiveSkillFrame.SetActive(false);
        var AllBuffs = warrior.Buffs;
        if (warrior.GetDefaultActiveSkill() != null) {
            ActiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.GetDefaultActiveSkill().ID) as Sprite;
            ActiveSkillFrame.SetActive(true);
        }
        FC.For(AllBuffs.Length, (i) =>
        {
            if (AllBuffs[i] is PatternSkill)
            {
                ActiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + AllBuffs[i].ID) as Sprite;
                ActiveSkillFrame.SetActive(true);
            }
            else if(AllBuffs[i] is ISkillWithPassiveSkill)
            {
                PassiveSkill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + AllBuffs[i].ID) as Sprite;
                PassiveSkillFrame.SetActive(true);
            }
        });
    }

    // 显示人物信息栏左下角buff栏
    public void UpDateBUFF(Warrior warrior)
    {
        bool[] isBUFFPicTaken = new bool[BUFFPic.Length];
        FC.For(BUFFPic.Length, (w) => BUFFPic[w].SetActive(false));

        foreach (var buff in warrior.Buffs)
        {
            if (BuffDisplayList.BuffsToDisplay.Contains((name)=> { return name.Equals(buff.ID); }))
            {
                for (int j = 0; j < BUFFPic.Length; j++)
                {
                    if (!isBUFFPicTaken[j])
                    {
                        BUFFPic[j].SetActive(true);
                        BUFFPic[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/BUFFs/" + buff.ID) as Sprite;
                        isBUFFPicTaken[j] = true;
                        break;
                    }
                }
            }
        };
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
                activeSkillName.text = activeSkill.DisPlayName();
                energyTrigger.text = activeSkill.EnergyCost.ToString() + " 能量";
                activeSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.GetDefaultActiveSkill().ID) as Sprite;
                ActiveSkillDescription.text = activeSkill.SkillDescription();
            }
            else // 如果是 patternskill
            {
                var AllBuffs = warrior.Buffs;
                FC.For(AllBuffs.Length, (i) =>
                {
                    if (AllBuffs[i] is PatternSkill)
                    {
                        var patternSkill = AllBuffs[i];
                        patternTrigger.SetActive(true);
                        energyTrigger.gameObject.SetActive(false);
                        activeSkillName.text = patternSkill.DisPlayName();
                        activeSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + AllBuffs[i].ID) as Sprite;
                        ActiveSkillDescription.text = patternSkill.SkillDescription();

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
                if (Buffs[i] is ISkillWithPassiveSkill)
                {
                    passiveSkillName.text = Buffs[i].DisPlayName();
                    passiveSkillIcon.sprite = Resources.Load<Sprite>("UI/Skill/" + Buffs[i].ID) as Sprite;
                    PassiveSkillDescription.text = Buffs[i].SkillDescription();
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

    public Dictionary<string, object> GetWarriorInfo(Warrior warrior)
    {
        Dictionary<string, object> warriorInfo = new Dictionary<string, object>();
        warriorInfo["AttackValue"] = warrior.ATK;
        warriorInfo["ArmorValue"] = warrior.ARM;
        warriorInfo["MagicValue"] = warrior.POW;
        warriorInfo["RESValue"] = warrior.RES;
        warriorInfo["ShieldValue"] = warrior.ES;
        warriorInfo["MaxShieldValue"] = warrior.MaxES;
        warriorInfo["HPValue"] = warrior.HP;
        warriorInfo["MaxHPValue"] = warrior.MaxHP;
        return warriorInfo;
    }
}

