using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ValuableType
{
    // Can't be a checklist item, but still gives points
    Money,
    // Checklist Items
    Trophy,
    Figurine,
    Cellphone,
    Briefcase,
    VhsTape,
    FloppyDisk

}

// An Interactable item that awards points when interacted with.
public class ValuableItem : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int scoreValue = 0;
    [SerializeField] private ValuableType type = ValuableType.Money;

    [Header("References")]
    [SerializeField] private Score scoreScript;
    [SerializeField] private ChecklistController checklistController;

    public void Start()
    {
        // Initialize References if not already set.
        if (scoreScript == null)
        {
            scoreScript = GameObject.FindWithTag("Canvas").GetComponentInChildren<Score>();
        }

        if (checklistController == null)
        {
            checklistController = GameObject.FindWithTag("Canvas").GetComponentInChildren<ChecklistController>();
        }
    }

    public ValuableType GetValuableType()
    {
        return type;
    }

    public void Interact()
    {
        // Increase the score by the valuable's score value.
        scoreScript.IncScore(scoreValue);

        // Update the checklsit if the valuable is a checklist item.
        checklistController.CheckValuable(this);
        
        //Play sound
        //SoundManager.PlaySound(gameObject, SoundEffect.Drop_Regular, 0.5f);
        //SoundManager.PlaySound(gameObject, SoundEffect.Inventory_Collect, 0.5f);

        Destroy(gameObject);
    }
}
