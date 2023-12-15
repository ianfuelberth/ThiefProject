using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivatorType
{
    // Activates on single F press - only 1 state/action
    PulseActivator,
    // Activates on single F press - toggles between 2 states/actions
    ToggleActivator,
    // Begins Timer on F press, activates after time has passed. Movement breaks channel.
    ChannelActivator
}

public abstract class Activator : MonoBehaviour, IInteractable
{
    public abstract ActivatorType Type { get; }
    public abstract GameObject[] ActivatorTargets { get; }
    public abstract int State { get; set; }

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public abstract void Interact();

    public void ActivateTargets()
    {
        if (ActivatorTargets.Length > 0)
        {
            foreach (GameObject activatorObject in ActivatorTargets)
            {
                IActivate activateScript = activatorObject.GetComponent<IActivate>();
                if (activateScript != null)
                {
                    activateScript.Activate(State);
                }
            }
        }
    }
}
