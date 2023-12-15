using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Component to allow interaction with the Escape Van to complete the mission. Added once the escape condiitons are met.
public class EscapeVan : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        EscapeController escapeController = GetComponent<EscapeController>();
        if (escapeController.canEscape && !escapeController.timesUp)
        {
            escapeController.EscapeSuccess();
        }

    }

}

