using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public enum ToolType
{   
    // Empty Tool Slot
    NoTool,
    // Throwables
    DefaultThrowable,
    Bone,
    // Experimental
    Rope,
    Grapple,

}

public abstract class Tool : MonoBehaviour, IInteractable
{
    public abstract ToolType ToolType { get; }
    public abstract string ToolName { get; }
    public abstract int Count { get; set; }

    protected Camera playerCam;
    protected GameObject player;

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCam = GameObject.FindGameObjectWithTag("PlayerCameraHolder").GetComponentInChildren<Camera>();
    }

    public virtual void FixedUpdate()
    {
        
    }

    #region ToolActions
    public void Interact()
    {
        Equip();
    }

    public virtual void Equip()
    {
        if (ToolType != ToolType.NoTool)
        {
            playerCam.GetComponent<ToolbeltController>().Equip(gameObject);
        }
    }

    public virtual void Drop()
    {
        if (ToolType != ToolType.NoTool)
        {
            playerCam.GetComponent<ToolbeltController>().Drop(gameObject);
        }
    }

    public abstract void PrimaryUse();
    public abstract void SecondaryUse();
    #endregion
}