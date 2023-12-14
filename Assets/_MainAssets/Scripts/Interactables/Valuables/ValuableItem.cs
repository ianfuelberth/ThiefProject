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
        scoreScript.IncScore(scoreValue);
        checklistController.CheckValuable(this);
        //Play sound
        //SoundManager.PlaySound(gameObject, SoundEffect.Drop_Regular, 0.5f);
        //SoundManager.PlaySound(gameObject, SoundEffect.Inventory_Collect, 0.5f);

        Destroy(gameObject);
    }
}
