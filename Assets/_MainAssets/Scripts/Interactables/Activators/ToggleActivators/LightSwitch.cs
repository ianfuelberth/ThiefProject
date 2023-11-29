using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : ToggleActivator
{
    [Header("Settings")]
    [SerializeField] private GameObject[] activatorTargets = null;
    [SerializeField] private int state = 0;
    
    public override GameObject[] ActivatorTargets { get { return activatorTargets; } }
    public override int State 
    { 
        get { return state; } 
        set { state = value; } 
    }

    public override void Activate(int state)
    {
        if (state == 0)
        {
            State = 1;
            // Debug.Log("LightSwitch: On");

        }
        else
        {
            State = 0;
            // Debug.Log("LightSwitch: Off");
        }
    }

}
