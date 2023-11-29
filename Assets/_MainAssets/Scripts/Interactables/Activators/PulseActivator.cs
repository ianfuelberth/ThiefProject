using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PulseActivator : Activator, IActivate
{
    public override ActivatorType Type { get { return ActivatorType.PulseActivator; } }
    public override abstract GameObject[] ActivatorTargets { get; }

    public override int State { get { return 0; } set { } }

    public override void Interact()
    {
        Activate(State);
        ActivateTargets();
    }

    public abstract void Activate(int state);
    
    /*
    private void ActivateTargets()
    {
        if (ActivatorTargets.Length > 0)
        {
            foreach (GameObject activatorObject in ActivatorTargets)
            {
                IActivate activateScript = activatorObject.GetComponent<IActivate>();
                if (activateScript != null)
                {
                    activateScript.Activate();
                }
            }
        }
    }\
    */
}
