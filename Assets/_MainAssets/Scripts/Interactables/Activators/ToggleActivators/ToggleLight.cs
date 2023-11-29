using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ToggleLight : MonoBehaviour, IActivate
{
    [Header("References")]
    [SerializeField] private GameObject lightSource;

    void Start()
    {
        lightSource.SetActive(false);
    }


    public void Activate(int state)
    {
        if (state == 0)
        {
            lightSource.SetActive(false);
        }
        else
        {
            lightSource.SetActive(true);
        }
    }
}
