using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

