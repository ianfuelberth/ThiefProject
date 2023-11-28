using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WheelMenuScript : MonoBehaviour
{
    private Vector2 normalizedMousePosition;
    private float currentAngle;

    [Header("Info")]
    private int selection = 0;
    private int prevSelection = 1;
    public bool menuIsActive = false;
    public string selectedItemName = "Empty";
    public int selectedItemCount = 0;
    public Image selectedItemIcon;
    
    [Header("References")]
    public GameObject textDisplay;
    public GameObject[] menuItems;
    public ToolbeltController toolbelt;
   
    private MenuItemScript menuItemScript;
    private MenuItemScript prevMenuItemScript;

    private void Start()
    {
        // toolbelt = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ToolbeltController>();
        DisableMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(PlayerKeybinds.TOOL_MENU_KEY))
        {
            EnableMenu();
        }
        
        if (Input.GetKeyUp(PlayerKeybinds.TOOL_MENU_KEY))
        {
            // set player's active toolbelt index to new selection
            toolbelt.SetActiveToolIndex(selection);

            DisableMenu();
        }

        if (menuIsActive)
        {
            normalizedMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);

            currentAngle = Mathf.Atan2(-normalizedMousePosition.y, normalizedMousePosition.x) * Mathf.Rad2Deg;
            currentAngle = (((currentAngle + 360 + 90 + 22.5f) % 360));

            selection = (int)(currentAngle / (360 / 8));
            
            if (selection != prevSelection)
            {
                prevMenuItemScript = menuItems[prevSelection].GetComponent<MenuItemScript>();
                prevMenuItemScript.Deselect();
                prevSelection = selection;

                menuItemScript = menuItems[selection].GetComponent<MenuItemScript>();
                menuItemScript.Select();

                Tool selectedTool = toolbelt.GetToolAtIndex(selection);
                selectedItemName = selectedTool.ToolName;
                selectedItemCount = selectedTool.Count;

                textDisplay.GetComponent<MenuDisplayScript>().UpdateDisplay(selectedItemName, selectedItemCount);
            }
        }
        
    }

    public void EnableMenu()
    {
        menuIsActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        textDisplay.GetComponent<MenuDisplayScript>().EnableDisplay();

        foreach (GameObject menu in menuItems)
        {
            menu.GetComponent<MenuItemScript>().EnableMenu();
        }
    }

    public void DisableMenu() 
    {
        menuIsActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        textDisplay.GetComponent<MenuDisplayScript>().DisableDisplay();

        foreach (GameObject menu in menuItems)
        {
            menu.GetComponent<MenuItemScript>().DisableMenu();
        }
    }

}
