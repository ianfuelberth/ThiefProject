using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : PulseActivator
{
    [Header("Settings")]
    [SerializeField] private GameObject[] activatorTargets = null;

    public override GameObject[] ActivatorTargets { get { return activatorTargets; } }

    public override void Activate(int state)
    {
        Debug.Log("Activated Test Button");
    }
}
