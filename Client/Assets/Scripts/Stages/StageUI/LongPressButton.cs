using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CharacterInfoUI CharacterInfoUI;
    private float delay =1.0f;

    // 按钮是否是按下状态  
    private bool isDown = false;

    // 按钮最后一次是被按住状态时候的时间  
    private float lastIsDownTime;  

    // Update is called once per frame
    void Update()
    {
        if (isDown)
        {
            if (Time.time - lastIsDownTime > delay)
            {
                CharacterInfoUI.ShowSkillPanel();
                isDown = false;
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        lastIsDownTime = Time.time;
    }
  
    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }

}
