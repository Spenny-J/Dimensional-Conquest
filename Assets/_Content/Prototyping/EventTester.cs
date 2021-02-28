using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTester : MonoBehaviour
{
    public UnityEvent onPressQ;
    public UnityEvent onPressW;
    public UnityEvent onPressE;
    public UnityEvent onPressR;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            onPressQ.Invoke();
        if (Input.GetKeyDown(KeyCode.W))
            onPressW.Invoke();
        if (Input.GetKeyDown(KeyCode.E))
            onPressE.Invoke();
        if (Input.GetKeyDown(KeyCode.R))
            onPressR.Invoke();
    }

}
