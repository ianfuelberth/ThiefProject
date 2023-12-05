using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

    [Header("Outline By Proximity")]
    public bool outlineByProx = false;
    public float outlineProxRange = 3.0f;
    public Color outlineProxColor = Color.white;
    public float outlineProxWidth = 1.0f;

    [Header("Outline By Raycast")]
    public bool outlineByCast = true;
    public float outlineCastRange = 3.0f;
    public Color outlineCastColor = Color.yellow;
    public float outlineCastWidth = 2.0f;
    
    // Other
    private LayerMask nonHeld;


    private void Start()
    {
        ReinitializeOutlines();

        nonHeld = ~(1 << LayerMask.NameToLayer("holdLayer"));
    }

    void FixedUpdate()
    {
        ClearActiveOutlines();

        if (outlineByProx == true)
        {
            AddOutlineByProximity();
        }

        if (outlineByCast == true)
        {
            AddOutlineByRaycast();
        }
        
    }

    private void ClearActiveOutlines()
    {
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

        foreach(GameObject obj in interactableObjects)
        {
            if (obj.GetComponent<Outline>() != null)
            {
                obj.GetComponent<Outline>().enabled = false;
            }
        }
    }

    private void ReinitializeOutlines()
    {
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (GameObject obj in interactableObjects)
        {
            if (obj.GetComponent<Outline>() != null)
            {
                obj.GetComponent<Outline>().enabled = false;
                Destroy(obj.GetComponent<Outline>());
            }
        }
    }

    private void AddOutlineByRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, outlineCastRange, nonHeld))
        {
            GameObject hitObj = hit.transform.gameObject;

            if (hitObj.CompareTag("Interactable"))
            {
                Outline outline;

                if (hitObj.GetComponent<Outline>() != null)
                {
                    outline = hitObj.GetComponent<Outline>();
                }
                else
                {
                    outline = hitObj.AddComponent<Outline>();
                }
                
                outline.enabled = true;
                outline.OutlineColor = outlineCastColor;
                outline.OutlineWidth = outlineCastWidth;

            }
        }
    }

    private void AddOutlineByProximity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, outlineProxRange, nonHeld);
        if (hitColliders.Length > 0)
        {
            foreach (Collider collider in hitColliders)
            {
                GameObject hitObj = collider.gameObject;

                if (hitObj.CompareTag("Interactable"))
                {
                    Outline outline;

                    if (hitObj.GetComponent<Outline>() != null)
                    {
                        outline = hitObj.GetComponent<Outline>();
                    }
                    else
                    {
                        outline = hitObj.AddComponent<Outline>();
                    }

                    outline.enabled = true;
                    outline.OutlineColor = outlineProxColor;
                    outline.OutlineWidth = outlineProxWidth;
                }
            }
        }
        
    }


}
