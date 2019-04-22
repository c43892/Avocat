using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameDriver : MonoBehaviour
{
    protected void Awake()
    {
        GameCore.Instance.Initialize();
    }

    protected void Start()
    {
        Application.runInBackground = true;
    }

    public void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var dtMs = (int)(dt * 1000);
        GameCore.Instance.RunOneFrame(dtMs);
    }
}
