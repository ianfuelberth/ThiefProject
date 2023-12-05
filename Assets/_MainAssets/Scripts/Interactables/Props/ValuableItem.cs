using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValuableItem : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int scoreValue = 0;
    public bool isChecklistItem = false;

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
