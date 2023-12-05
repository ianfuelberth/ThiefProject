using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Transform activeItemPos;

    [Header("Settings")]
    public float pickUpRange = 3.0f;
    public bool debugMode = true;

    [Header("Details")]
    [SerializeField] private static bool hasActiveItem;
    [SerializeField] private bool canDrop = true;
    private int layerNum;
    private LayerMask nonHeld;

    [Header("Held Item")]
    private GameObject heldItem;
    // [SerializeField] private Vector3 heldVelocity;
    // public bool isCollidingWithPlayer = false;
    private int itemCount = 0;
    private ThrowableItem itemScript;
    private Rigidbody heldItemRb;

    private void Start()
    {
        layerNum = LayerMask.NameToLayer("holdLayer");

        // LayerMask containing all layers besides the "holdLayer". Helps ensure held item won't interfere with raycasts.
        nonHeld = ~(1 << LayerMask.NameToLayer("holdLayer"));
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, nonHeld))
            {
                if (Input.GetKey(PlayerKeybinds.INTERACT_KEY))
                {
                    Gizmos.DrawLine(transform.position, hit.transform.position);
                }
            }
        }
    }

    private void Update()
    {
        if (heldItem != null)
        {
            Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), true);
            /*
            heldVelocity = heldItemRb.velocity;
            isCollidingWithPlayer = itemScript.GetPlayerCollisionStatus();
            GameObject[] throwables = GameObject.FindGameObjectsWithTag("Interactable");
            foreach(GameObject throwableObj in throwables)
            {
                if (throwableObj.GetComponent<Collider>() != null)
                {
                    Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), throwableObj.GetComponent<Collider>(), true);
                }
            }
            */
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, nonHeld))
        {
            // Handle Item Pickup
            if (hit.transform.gameObject.GetComponent<ThrowableItem>())
            {
                if (Input.GetKey(PlayerKeybinds.INTERACT_KEY))
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
            
            // Handle Item Drop
            if (heldItem != null)
            {
                if (canDrop && Input.GetKeyUp(PlayerKeybinds.DROP_KEY))
                {
                    DisableClipping();
                    DropObject();
                }
            }
        }

        if (heldItem != null && hasActiveItem)
        {
            MoveObject();
            if (Input.GetKeyUp(PlayerKeybinds.SECONDARY_USE_KEY) && canDrop)
            {
                DisableClipping();
                ThrowObject();
            }
            else if (Input.GetKeyUp(PlayerKeybinds.DROP_KEY) && canDrop)
            {
                DisableClipping();
                DropObject();
            }
        }
    }

    private void PickUpObject(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody>() != null && obj.GetComponent<ThrowableItem>() != null) 
        {
            if (heldItem == null)
            {
                heldItem = obj;
                heldItem.layer = layerNum;

                itemScript = obj.GetComponent<ThrowableItem>();
                itemCount = itemScript.count;

                heldItemRb = obj.GetComponent<Rigidbody>();
                heldItemRb.isKinematic = true;
                // heldItem.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                // heldItemRb.transform.rotation = Quaternion.identity;
                heldItem.transform.parent = activeItemPos.transform;

                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;

                

                Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), true);

                hasActiveItem = true;
            }
            else
            {
                // check if picking up object of same type
                ThrowableItem targetScript = obj.GetComponent<ThrowableItem>();
                string targetType = targetScript.itemType;
                // Debug.Log("Held: " + itemScript.itemType + ", Target: " +  targetType);
                if (Equals(targetType, itemScript.itemType))
                {
                    itemCount += targetScript.count;
                    Destroy(obj);
                }
                else
                {
                    while(itemCount > 0)
                    {
                        DropObject();
                    }
                   
                    heldItem = obj;
                    heldItem.layer = layerNum;

                    itemScript = obj.GetComponent<ThrowableItem>();
                    itemCount = itemScript.count;

                    heldItemRb = obj.GetComponent<Rigidbody>();
                    heldItemRb.isKinematic = true;
                    heldItem.transform.parent = activeItemPos.transform;

                    Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), true);

                    heldItem.transform.localRotation = Quaternion.identity;

                    hasActiveItem = true;
                }
            }
        }
    }

    private void DropObject()
    {
        if (itemCount >= 2)
        {
            GameObject dropObj = Instantiate(heldItem, activeItemPos.position, transform.rotation);

            //Physics.IgnoreCollision(dropObj.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), false);
            QueueReEnableCollisionsWithPlayer();
            Physics.IgnoreCollision(dropObj.GetComponent<Collider>(), heldItem.GetComponent<Collider>(), false);

            Rigidbody dropObjRb = dropObj.GetComponent<Rigidbody>();
            dropObj.layer = 0;
            dropObjRb.isKinematic = false;
            dropObj.transform.parent = null;

            itemCount--;
        }
        else
        {
            //Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), false);
            QueueReEnableCollisionsWithPlayer();
            heldItem.layer = 0;
            heldItemRb.isKinematic = false;
            heldItem.transform.parent = null;
            heldItem = null;
            // heldItemRb = null;

            itemScript = null;
            itemCount = 0;
        }
    }

    private void ThrowObject()
    {
        // create copy of item to throw if player has more than one of the held item
        if (itemCount >= 2)
        {
            GameObject throwObj = Instantiate(heldItem, activeItemPos.position, transform.rotation);

            // Physics.IgnoreCollision(throwObj.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), false);
            QueueReEnableCollisionsWithPlayer();
            Physics.IgnoreCollision(throwObj.GetComponent<Collider>(), heldItem.GetComponent<Collider>(), false);

            Rigidbody throwObjRb = throwObj.GetComponent<Rigidbody>();
            throwObj.layer = 0;
            throwObjRb.isKinematic = false;
            throwObj.transform.parent = null;

            throwObjRb.velocity = player.GetComponentInChildren<Rigidbody>().velocity;
            throwObjRb.AddForce(transform.forward * itemScript.throwForwardForce, ForceMode.Impulse);
            throwObjRb.AddForce(transform.up * itemScript.throwUpwardForce, ForceMode.Impulse);
            float random = Random.Range(-1f, 1f);
            throwObjRb.AddTorque(new Vector3(random, random, random) * 10);

            itemCount--;
        }
        else
        {
            // Physics.IgnoreCollision(heldItem.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), false);
            QueueReEnableCollisionsWithPlayer();
            heldItem.layer = 0;
            heldItemRb.isKinematic = false;
            heldItem.transform.parent = null;


            // calculate direction
            /*
            Vector3 forceDir = transform.forward;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 500f))
            {
                forceDir = (hit.point - activeItemPos.position).normalized;
            }
            */

            // add force
            heldItemRb.velocity = player.GetComponent<Rigidbody>().velocity;
            heldItemRb.AddForce(transform.forward * itemScript.throwForwardForce, ForceMode.Impulse);
            heldItemRb.AddForce(transform.up * itemScript.throwUpwardForce, ForceMode.Impulse);
            float random = Random.Range(-1f, 1f);
            heldItemRb.AddTorque(new Vector3(random, random, random) * 10);

            heldItem = null;
            hasActiveItem = false;

            itemScript = null;
            itemCount = 0;
        }
    }

    // TODO: dropped/thrown object should ignore collisions with player until after 0.01 seconds of them no longer colliding
    private void QueueReEnableCollisionsWithPlayer()
    {
        itemScript.QueueReEnableCollisionWithPlayer();
    }

    private void MoveObject()
    {
        heldItem.transform.position = activeItemPos.transform.position;
    }

    private void DisableClipping()
    {
        var clipRange = Vector3.Distance(heldItem.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {
            heldItem.transform.position = transform.position + new Vector3(0f, -0.1f, -0f);
        }
    }    

}
