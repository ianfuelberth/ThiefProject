using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemScript : MonoBehaviour
{
    [Header("Menu Settings")]
    public Image basePanel;
    public Color baseColor;
    public Color baseHighlightColor;

    public Image edgePanel;
    public Color edgeColor;
    public Color edgeHighlightColor;

    [Header("Menu Item Info")]
    public bool selected = false;
    public Image icon;

    void Start()
    {
        basePanel.color = baseColor;
        edgePanel.color = edgeColor;
    }

    public void Select()
    {
        basePanel.color = baseHighlightColor;
        edgePanel.color = edgeHighlightColor;
        selected = true;
    }

    public void Deselect()
    { 
        basePanel.color = baseColor;
        edgePanel.color = edgeColor;
        selected = false;
    }

    public void EnableMenu()
    {
        basePanel.enabled = true;
        edgePanel.enabled = true;
        icon.enabled = true;
    }

    public void DisableMenu()
    {
        basePanel.enabled = false;
        edgePanel.enabled = false;
        icon.enabled = false;
    }

}
