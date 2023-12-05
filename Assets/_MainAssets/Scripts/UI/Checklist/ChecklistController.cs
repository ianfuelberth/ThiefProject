using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ChecklistController : MonoBehaviour
{
    [Header("Checklist Items")]
    [SerializeField]
    private ChecklistItem[] checklistItems;
    public const int NUM_ITEMS = 5;
    [SerializeField]
    private bool isCompleted = false;

    [Header("References")]
    [SerializeField]
    private GameObject EscapeAvailableText;
    [SerializeField]
    private EscapeController escapeController;
    
    [SerializeField]
    private ChecklistItem defaultChecklistItem;
    
    // Start is called before the first frame update
    void Start()
    {
        ToggleEscapeMessage(false);

        if (escapeController == null)
        {
            escapeController = GameObject.FindGameObjectWithTag("Escape").GetComponent<EscapeController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetIsCompleted()
    {
        isCompleted = IsCompletedChecklist();

        if (isCompleted)
        {
            ToggleEscapeMessage(true);
            escapeController.ChecklistFilled();
        }

        return isCompleted;
    }

    private void ToggleEscapeMessage(bool isActive)
    {
        float transparency = 0f;
        if (isActive)
        {
            transparency = 1f;
        }
        EscapeAvailableText.GetComponent<CanvasGroup>().alpha = transparency;
    }

    public void CheckValuable(ValuableItem valuable)
    {
        foreach (ChecklistItem item in checklistItems)
        {
            if (item.GetCollectedStatus() == false && valuable.GetValuableType() == item.GetValuableType()) 
            {
                item.MarkAsCollected();
                break;
            }
        }

        isCompleted = IsCompletedChecklist();
        GetIsCompleted();
    }

    private bool IsCompletedChecklist()
    {
        bool isCompleted = true;

        foreach (ChecklistItem item in checklistItems)
        {
            if (item.GetCollectedStatus() == false)
            {
                isCompleted = false;
                break;
            }
        }

        return isCompleted;
    }

    
}
