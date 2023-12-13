using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableTool : Tool
{
    [Header("Settings")]
    [SerializeField] private ToolType toolType = ToolType.DefaultThrowable;
    [SerializeField] private string toolName = "Default Throwable";
    [SerializeField] private int count = 1;
    [SerializeField] private Image icon;
    [SerializeField] private float throwForwardForce = 10f;
    [SerializeField] private float throwUpwardForce = 2f;

    /*
    private bool isCollidingWithPlayer = false;
    private bool isWaitingToEnableCollisions = false;
    private float waitTime = 0;
    private GameObject heldTool;
    */
    public override ToolType ToolType { get { return toolType; } }
    public override string ToolName { get { return toolName; } }
    private bool thrown = false;
    private Navigation navigationScript;

    public override int Count
    {
        get { return count; }
        set { count = Mathf.Max(0, value); }
    }

    public override void Start()
    {
        base.Start();
        // navigationScript = GameObject.FindWithTag("AI").GetComponent<Navigation>();

        // isWaitingToEnableCollisions = false;
    }

    public override void FixedUpdate()
    {
        /*
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
        */
    }

    #region ToolActions
    public override void PrimaryUse()
    {
        Debug.Log("Tool: ThrowableTool.PrimaryUse() does nothing.");
    }

    public override void SecondaryUse()
    {
        thrown = true;
        playerCam.GetComponentInChildren<ToolbeltController>().Throw(gameObject, throwForwardForce, throwUpwardForce);
    }
    #endregion

    private void OnCollisionEnter(Collision collision) {
        if(thrown)
        {
            if (navigationScript != null)
            {
                navigationScript.Distract(transform.position);
                thrown = false;
            }
            

        }
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    /*
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
    */
}
