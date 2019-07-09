using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostHP : MonoBehaviour
{
    public GameObject HP;
    public bool readyToStart;
    public float timer =0f;

    private void Update()
    {
        if (readyToStart)
        {
            if (timer <= 1f)
            {
                SetHPLostAnimation();
                timer = timer + Time.deltaTime;
            }
            else
                Destroy(gameObject);
        }       
    }

    public void SetHPLost(int dhp)
    {
        var hp = HP.GetComponent<TextMesh>();
        hp.text = dhp.ToString();
        readyToStart = true;
    }

    public void SetHPLostAnimation()
    {
        gameObject.transform.Translate(new Vector3(0, 1, 0)*0.5f*Time.deltaTime, Space.World);
    }
}
