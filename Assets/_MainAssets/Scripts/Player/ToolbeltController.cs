using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbeltController : MonoBehaviour
{
    [Header("Toolbelt")]
    public GameObject[] toolObjects;
    public int activeToolIndex;

    [Header("References")]
    public Transform toolbelt;
    public GameObject noToolPrefab;
    public GameObject player;
    

    //---HELD ITEM---//
    /*
    private GameObject heldObj;
    private Tool heldTool;
    private Rigidbody heldRb;
    private int heldCount = 0;
    */
    //---DETAILS---//
    private bool canDrop = true;
    private int layerNum;
    private LayerMask nonHeld;

    private void Start()
    {
        layerNum = LayerMask.NameToLayer("holdLayer");
        nonHeld = ~(1 << LayerMask.NameToLayer("holdLayer"));

        toolObjects = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            
            toolObjects[i] = Instantiate(noToolPrefab, toolbelt);
            toolObjects[i].SetActive(false);
        }

        activeToolIndex = 0;
        toolObjects[activeToolIndex].SetActive(true);
    }

    private void Update()
    {
        if (HasActiveTool())
        {
            Physics.IgnoreCollision(GetActiveTool().gameObject.GetComponent<Collider>(), gameObject.GetComponentInChildren<Collider>(), true);

            MoveActiveObject();

            if (Input.GetKeyUp(PlayerKeybinds.PRIMARY_USE_KEY))
            {
                GetActiveTool().PrimaryUse();
            }
            else if (Input.GetKeyUp(PlayerKeybinds.SECONDARY_USE_KEY))
            {
                GetActiveTool().SecondaryUse();
            }
            else if (Input.GetKeyUp(PlayerKeybinds.DROP_KEY))
            {
                GetActiveTool().Drop();
            }
        }
    }

    public void Equip(GameObject toolObj)
    {
        Tool tool = toolObj.GetComponent<Tool>();
        if (tool.ToolType != ToolType.NoTool && toolObj.GetComponent<Rigidbody>() != null)
        {
            // If not currently holding tool
            if (!HasActiveTool())
            {
                toolObj.layer = layerNum;

                Rigidbody heldRb = toolObj.GetComponent<Rigidbody>();
                heldRb.isKinematic = true;
                toolObj.transform.localRotation = Quaternion.identity;

                Destroy(toolbelt.GetChild(activeToolIndex).gameObject);
                GameObject newToolInstance = Instantiate(toolObj, toolbelt.transform);
                newToolInstance.transform.SetSiblingIndex(activeToolIndex);
                Destroy(toolObj);

                toolObjects[activeToolIndex] = newToolInstance;
                Physics.IgnoreCollision(newToolInstance.GetComponent<Collider>(), player.GetComponentInChildren<Collider>(), true);
            }
            // If already holding tool of same type
            else if (GetActiveTool().ToolType == tool.ToolType)
            {
                GetActiveTool().Count += tool.Count;
                Destroy(toolObj);
            }
            // If already holding tool of different type
            else
            {
                Debug.Log("Cannot pick up item of different type to currently held");
            }
        }
    }

    public void Drop(GameObject toolObj)
    {
        if (!canDrop) { return; }
        DisableClipping();
        
        Tool tool = toolObj.GetComponent<Tool>();

        if (tool.Count > 1)
        {
            GameObject dropObj = Instantiate(toolObj, toolbelt.position, transform.rotation);
            dropObj.GetComponent<Tool>().Count -= 1;

            QueueReEnableCollisionsWith(dropObj.GetComponent<Tool>(), GetActiveTool().gameObject);
            Physics.IgnoreCollision(dropObj.GetComponent<Collider>(), GetActiveTool().gameObject.GetComponent<Collider>(), true);

            Rigidbody dropObjRb = dropObj.GetComponent<Rigidbody>();
            dropObj.layer = 0;
            dropObjRb.isKinematic = false;
            dropObj.transform.parent = null;

            GetActiveTool().Count -= 1;
        }
        else
        {
            GameObject dropObj = GetActiveTool().gameObject;

            QueueReEnableCollisionsWith(dropObj.GetComponent<Tool>(), GetActiveTool().gameObject);
            Physics.IgnoreCollision(dropObj.GetComponent<Collider>(), GetActiveTool().gameObject.GetComponent<Collider>(), true);

            Rigidbody dropObjRb = dropObj.GetComponent<Rigidbody>();
            dropObj.layer = 0;
            dropObjRb.isKinematic = false;
            dropObj.transform.parent = null;

            EmptyToolSlot();
        }

    }

    public void Throw(GameObject toolObj, float throwForwardForce, float throwUpwardForce)
    {
        if (!canDrop) { return; }
        DisableClipping();

        ThrowableTool tool = toolObj.GetComponent<ThrowableTool>();

        if (tool.Count > 1)
        {
            GameObject throwObj = Instantiate(toolObj, toolbelt.position, transform.rotation);
            throwObj.GetComponent<Tool>().Count -= 1;

            QueueReEnableCollisionsWith(throwObj.GetComponent<ThrowableTool>(), GetActiveTool().gameObject);
            Physics.IgnoreCollision(throwObj.GetComponent<Collider>(), GetActiveTool().gameObject.GetComponent<Collider>(), true);

            Rigidbody throwObjRb = throwObj.GetComponent<Rigidbody>();
            throwObj.layer = 0;
            throwObjRb.isKinematic = false;
            throwObj.transform.parent = null;

            throwObjRb.velocity = player.GetComponentInChildren<Rigidbody>().velocity;
            throwObjRb.AddForce(transform.forward * throwForwardForce, ForceMode.Impulse);
            throwObjRb.AddForce(transform.up * throwUpwardForce, ForceMode.Impulse);
            float random = Random.Range(-1f, 1f);
            throwObjRb.AddTorque(new Vector3(random, random, random) * 10);

            GetActiveTool().Count -= 1;
        }
        else
        {
            GameObject throwObj = GetActiveTool().gameObject;

            QueueReEnableCollisionsWith(throwObj.GetComponent<ThrowableTool>(), GetActiveTool().gameObject);
            Physics.IgnoreCollision(throwObj.GetComponent<Collider>(), GetActiveTool().gameObject.GetComponent<Collider>(), true);

            Rigidbody throwObjRb = throwObj.GetComponent<Rigidbody>();
            throwObj.layer = 0;
            throwObjRb.isKinematic = false;
            throwObj.transform.parent = null;

            throwObjRb.velocity = player.GetComponentInChildren<Rigidbody>().velocity;
            throwObjRb.AddForce(transform.forward * throwForwardForce, ForceMode.Impulse);
            throwObjRb.AddForce(transform.up * throwUpwardForce, ForceMode.Impulse);
            float random = Random.Range(-1f, 1f);
            throwObjRb.AddTorque(new Vector3(random, random, random) * 10);

            EmptyToolSlot();
        }
    }

    private void EmptyToolSlot()
    {
        GameObject toolObj = noToolPrefab;
        toolObj.layer = layerNum;
        
        GameObject newToolInstance = Instantiate(toolObj, toolbelt.transform);
        newToolInstance.transform.SetSiblingIndex(activeToolIndex);

        toolObjects[activeToolIndex] = newToolInstance;
    }

    private void MoveActiveObject()
    {
        GetActiveTool().gameObject.transform.position = toolbelt.transform.position;
    }

    private void DisableClipping()
    {
        var clipRange = Vector3.Distance(GetActiveTool().gameObject.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {
            GetActiveTool().gameObject.transform.position = transform.position + new Vector3(0f, -0.1f, -0f);
        }
    }

    private void QueueReEnableCollisionsWith(Tool tool, GameObject heldObject)
    {
        tool.QueueReEnableCollisionWith(heldObject);
    }

    public bool HasActiveTool()
    {
        return toolObjects[activeToolIndex].GetComponent<Tool>().ToolType != ToolType.NoTool;
    }

    public Tool GetActiveTool()
    {
        return toolObjects[activeToolIndex].GetComponent<Tool>();
    }

    public Tool GetToolAtIndex(int index)
    {
        return toolObjects[index].GetComponent<Tool>();
    }

    public void SetActiveToolIndex(int index)
    {
        // Change 'previously' activeObject
        GetActiveTool().gameObject.SetActive(false);
        
        activeToolIndex = Mathf.Clamp(index, 0, 7);

        GetActiveTool().gameObject.SetActive(true);
    }

    private void ResetActiveToEmpty()
    {
        toolObjects[activeToolIndex] = noToolPrefab;
    }


}
