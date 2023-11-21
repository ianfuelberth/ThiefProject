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
    Grapple

}

public abstract class Tool : MonoBehaviour, IInteractable
{
    public abstract ToolType ToolType { get; }
    public abstract string ToolName { get; }
    public abstract int Count { get; set; }

    protected Camera playerCam;
    protected GameObject player;

    private bool isCollidingWithPlayer = false;
    private bool isWaitingToEnableCollisions = false;
    private float waitTime = 0;
    private GameObject heldTool;

    public virtual void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (playerCam == null) playerCam = GameObject.FindGameObjectWithTag("PlayerCameraHolder").GetComponentInChildren<Camera>();
        // if (isWaitingToEnableCollisions == null) isWaitingToEnableCollisions = false;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        if (isWaitingToEnableCollisions && !isCollidingWithPlayer)
        {
            if (waitTime >= 0.6)
            {
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), false);

                if (heldTool != null)
                {
                    Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), heldTool.GetComponent<Collider>(), false);
                }

                isWaitingToEnableCollisions = false;
                waitTime = 0;
            }
            else
            {
                waitTime += Time.deltaTime;
            }
        }
    }

    public virtual void LateUpdate()
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


    #region PlayerCollision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    public bool GetPlayerCollisionStatus()
    {
        return isCollidingWithPlayer;
    }

    public void QueueReEnableCollisionWith(GameObject heldObject)
    {
        isWaitingToEnableCollisions = true;
        heldTool = heldObject;
    }
    #endregion
}