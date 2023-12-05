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

}


public class ValuableItem : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int scoreValue = 0;

    public ValuableType type = ValuableType.Money; 

    [Header("References")]
    [SerializeField]
    private Score scoreScript;

    public void Start()
    {
        if (scoreScript == null)
        {
            scoreScript = GameObject.FindWithTag("Canvas").GetComponentInChildren<Score>();
        }
    }

    public void Interact()
    {
        scoreScript.IncScore(scoreValue);
        Destroy(gameObject);
    }
}
