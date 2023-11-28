using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : Tool
{
    [Header("References")]
    [SerializeField] private GameObject lightSource;
    [SerializeField] private bool isOn = false;

    private int count = 1;

    public override ToolType ToolType { get { return ToolType.Flashlight; } }

    public override string ToolName { get { return "Flashlight"; } }

    public override int Count { get { return count; } set { count = value; } }

    public override void Start()
    {
        base.Start();
        lightSource.SetActive(false);
    }

    public override void PrimaryUse()
    {
        
    }

    public override void SecondaryUse()
    {
        if (!isOn)
        {
            isOn = true;
            lightSource.SetActive(true);
        }
        else
        {
            isOn = false;
            lightSource.SetActive(false);
        }
    }
}
