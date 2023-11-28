using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoTool : Tool
{
    [SerializeField] private Image icon;

    public override ToolType ToolType { get; } = ToolType.NoTool;
    public override string ToolName { get; } = "Empty";
    public override int Count { get; set; } = 0;

    public override void PrimaryUse()
    {
        Debug.Log("Tool: NoTool.PrimaryUse() does nothing.");
    }

    public override void SecondaryUse()
    {
        Debug.Log("Tool: NoTool.SecondaryUse() does nothing.");
    }
}
