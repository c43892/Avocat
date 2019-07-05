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
    public GameObject ItemUsage;
    public GameObject EndButton;

    public void UpdateSkill(Warrior warrior)
    {
        // 如果为敌军或者在空地则显示为地形改造按钮
        if (warrior == null || !(warrior is Hero))
        {
            UpdateItemUsage();
            return;
        }
            
        skill.SetActive(true);
        ItemUsage.SetActive(false);
        if (warrior.GetDefaultActiveSkill() is ActiveSkill) // 若为主动技能
        {
            skill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.GetDefaultActiveSkill().ID) as Sprite;
            energyHint.SetActive(true);
            skillHint.SetActive(false);

            // 如果是主动技能则使用button
            skill.GetComponent<Button>().enabled = true;
            energy.text = (warrior.GetDefaultActiveSkill() as ActiveSkill).EnergyCost.ToString();
        }
        else if (warrior.PatternSkill != null) // 若为pattern技能
        {
            skill.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Skill/" + warrior.PatternSkill.ID) as Sprite;
            energyHint.SetActive(false);
            skillHint.SetActive(true);

            // 如果是pattern技能则不使用button
            skill.GetComponent<Button>().enabled = false;

            // 先将显示技能提示区域变为不可见
            FC.For(skillCards.Length, (i) =>
            {
              skillCards[i].SetActive(false);
            });

            // 将技能提示栏的图片更换为pattern技能的出招顺序
            FC.For(warrior.PatternSkill.CardsPattern.Length, (i) =>
            {
                skillCards[i].SetActive(true);
                skillCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CardType/" + warrior.PatternSkill.CardsPattern[i]) as Sprite;
            });
        }
    }

    // 更新技能状态
    public void UpdateSkillState(int en, Warrior warrior)
    {
        UpdateSkill(warrior);
        var button = skill.GetComponent<Button>();
        if (!(warrior is Hero))
            return;
            
        if (!warrior.ActionDone && en >= int.Parse(energy.text) && !warrior.IsSkillReleased)
            button.interactable = true;
        else
            button.interactable = false;
    }

    // 显示地形改造按钮
    public void UpdateItemUsage() {
        skill.SetActive(false);
        ItemUsage.SetActive(true);
        energyHint.SetActive(false);
        skillHint.SetActive(false);
    }

    public void HideButton() {
        skill.SetActive(false);
        EndButton.SetActive(false);
    }

    public void ShowButton()
    {
        skill.SetActive(true);
        EndButton.SetActive(true);
    }
}
