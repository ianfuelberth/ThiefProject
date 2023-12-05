using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplayScript : MonoBehaviour
{
    [Header("References")]
    public Text displayText;
    public Text displayCount;

    [Header("Info")]
    public string selectedText = "null";
    public int selectedCount = -1;

    public void UpdateDisplay(string text, int count)
    {
        selectedText = text;
        displayText.text = selectedText;

        selectedCount = count;
        if (selectedCount > 0)
        {
            displayCount.text = selectedCount.ToString();
        }
        else
        {
            displayCount.text = "";
        }
    }

    public void EnableDisplay()
    {
        UpdateDisplay(selectedText, selectedCount);
    }

    public void DisableDisplay()
    {
        displayText.text = "";
        displayCount.text = "";
    }

}
