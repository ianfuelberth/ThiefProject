using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PulseActivator -- used for buttons.
public abstract class PulseActivator : Activator, IActivate
{
    // The ActivatorType of the Activator: PulseActivator
    public override ActivatorType Type { get { return ActivatorType.PulseActivator; } }
    
    // The ActivatorTargets that are activated by this Activator
    public override abstract GameObject[] ActivatorTargets { get; }

    // The state of the Activator.
    public override int State { get { return 0; } set { } }

    public override void Interact()
    {
        Activate(State);
        ActivateTargets();
    }

    // What the activator should do upon activation
    public abstract void Activate(int state);
    
}
