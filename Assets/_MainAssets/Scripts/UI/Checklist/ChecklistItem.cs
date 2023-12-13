using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistItem: MonoBehaviour
{
    [Header("Item Details")]
    [SerializeField]
    private ValuableType valuableType;
    [SerializeField]
    private bool isCollected = false;

    [Header("References")]
    [SerializeField]
    private TMP_Text itemText;
    [SerializeField]
    private TMP_Text strikethrough;

    

    

    public void InitializeItem(ValuableType type)
    {
        valuableType = type;
        isCollected = false;

        strikethrough.enabled = false;
        itemText.text = valuableType.ToString();
    }

    public ValuableType GetValuableType()
    {
        return valuableType;
    }

    public bool GetCollectedStatus()
    {
        return isCollected;
    }

    public void MarkAsCollected()
    {
        strikethrough.enabled = true;
        isCollected = true;
    }


}
