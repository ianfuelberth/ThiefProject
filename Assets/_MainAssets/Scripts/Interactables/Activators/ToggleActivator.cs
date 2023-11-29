using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleActivator : Activator, IActivate
{
    public override ActivatorType Type { get { return ActivatorType.ToggleActivator; } }
    public override abstract GameObject[] ActivatorTargets { get; }
    public override abstract int State { get; }

    public override void Interact()
    {
        Activate(State);
        ActivateTargets();
    }

    public abstract void Activate(int state);
}
