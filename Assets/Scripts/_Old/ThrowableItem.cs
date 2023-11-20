using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    [Header("Settings")]
    public string itemType = "empty";
    public float throwForwardForce = 10f;
    public float throwUpwardForce = 2f;
    public int count = 1;

    [Header("Player Collision")]
    [SerializeField] private bool isCollidingWithPlayer = false;
    [SerializeField] private bool isWaitingToEnableCollisions = false;
    [SerializeField] private float waitTime = 0;
    
    private Collider playerCollider;

    private void Start()
    {
        playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
        isWaitingToEnableCollisions = false;
    }

    private void Update()
    {
        

        if (isWaitingToEnableCollisions && isCollidingWithPlayer == false)
        {
            if (waitTime >= 0.6)
            {
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), GameObject.FindWithTag("Player").GetComponentInChildren<Collider>(), false);
                if (isCollidingWithPlayer)
                {
                    Debug.Log("Released and is Colliding With Player");
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

    public void QueueReEnableCollisionWithPlayer()
    {
        isWaitingToEnableCollisions = true;
    }
}
