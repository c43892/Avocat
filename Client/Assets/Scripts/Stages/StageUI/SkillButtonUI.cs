using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

public class SkillButtonUI : MonoBehaviour
{
    public GameObject skill;
    public GameObject skillHint;
    public GameObject energyHint;
    public Text energy;
    public GameObject[] skillCards;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateSkill(Warrior warrior)
    {
        skill.GetComponent<Image>().enabled = true;
        if (warrior.GetDefaultActiveSkill() is ActiveSkill)
        {
            skill.GetComponent<Image>().sprite = Resources.Load("UI/Skill/" + warrior.GetDefaultActiveSkill().Name, typeof(Sprite)) as Sprite;
            energyHint.SetActive(true);
            skillHint.SetActive(false);

            // 如果是主动技能可以点击
            skill.GetComponent<Button>().enabled = true;
            energy.text = (warrior.GetDefaultActiveSkill() as ActiveSkill).EnergyCost.ToString();
        }
        else if (warrior.PatternSkill != null)
        {
            skill.GetComponent<Image>().sprite = Resources.Load("UI/Skill/" + warrior.PatternSkill.Name, typeof(Sprite)) as Sprite;
            energyHint.SetActive(false);
            skillHint.SetActive(true);

            // 如果是pattern技能则不能点击
            skill.GetComponent<Button>().enabled = false;
            if (skillCards.Length >= warrior.PatternSkill.CardsPattern.Length)
            {
                FC.For(warrior.PatternSkill.CardsPattern.Length, (i) =>
                {
                     skillCards[i].SetActive(true);
                     skillCards[i].GetComponent<Image>().sprite = Resources.Load("UI/CardType/" + warrior.PatternSkill.CardsPattern[i], typeof(Sprite)) as Sprite;
                });
                if (skillCards.Length > warrior.PatternSkill.CardsPattern.Length)
                {
                    FC.For(warrior.PatternSkill.CardsPattern.Length, skillCards.Length, (i) =>
                     {
                         skillCards[i].SetActive(false);
                     });
                }

            }

        }
    }
    public void UpdateSkillState(int en)
    {
        if (en >= int.Parse(energy.text))
        {
            skill.GetComponent<Button>().interactable = true;
        }
        else
        {
            skill.GetComponent<Button>().interactable = false;
        }
    }

}
