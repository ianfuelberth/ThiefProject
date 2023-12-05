using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum KeyID
{ 
    DevMasterKey,
    DevUselessKey,
    Keycard_01
}


public class Key : Tool
{
    [Header("Settings")]
    [SerializeField] private ToolType toolType = ToolType.Key;
    [SerializeField] private string toolName = "Broken Key";
    [SerializeField] private KeyID keyID = KeyID.DevUselessKey;
    [SerializeField] private int count = 1;
    [SerializeField] private Image icon;

    public override ToolType ToolType { get { return toolType; } }
    public override string ToolName { get { return toolName; } }
    public override int Count
    {
        get { return count; }
        set { count = Mathf.Max(0, value); }
    }

    public bool IsCorrectKey(KeyID requiredKey)
    {
        if (keyID == requiredKey || keyID == KeyID.DevMasterKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void PrimaryUse()
    {
        // Debug.Log("Tool: Key.PrimaryUse() does nothing.");
    }

    public override void SecondaryUse()
    {
        // Debug.Log("Tool: Key.SecondaryUse() does nothing.");
    }

}
