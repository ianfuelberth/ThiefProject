using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum ActivatorType
{ 
    // Activates on single F press - only 1 state/action
    PulseActivator,
    // Activates on single F press - toggles between 2 states/actions
    ToggleActivator,
    // Begins Timer on F press, activates after time has passed. Movement breaks channel.
    ChannelActivator
}
*/

public abstract class ActivatorProp : MonoBehaviour, IInteractable
{
    /*
    [Header("Settings")]
    public ActivatorType ActivatorType = ActivatorType.PulseActivator;

    public bool isActive = false;
    public bool isChanneling = false;
    public float channelDuration = 5f;
    private float channelTime = 0;
    */

    public abstract GameObject[] ActivatorTargets { get; set; }

    [Header("References")]
    protected Camera playerCam;
    protected GameObject player;

    
    public virtual void Start()
    { 
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (playerCam == null) playerCam = GameObject.FindGameObjectWithTag("PlayerCameraHolder").GetComponentInChildren<Camera>();
    }

    public virtual void Update()
    {

    }

    public virtual void Interact()
    {
        ActivateTargets();
    }

    private void ActivateTargets()
    {
        foreach (GameObject activatorObject in ActivatorTargets)
        {
            IActivate activateScript = activatorObject.GetComponent<IActivate>();
            if (activateScript != null)
            {
                activateScript.Activate(0);
            }
        }
    }
}
