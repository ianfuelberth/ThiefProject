using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    private GameObject canvas;
    private Score scoreTrackerScript;
    [Header("References")]
    public Transform InteractSource;

    [Header("Settings")]
    public float interactRange = 4.0f;

    [Header("Outline By Proximity")]
    public bool outlineByProx = false;
    public float outlineProxRange = 4.0f;
    public Color outlineProxColor = Color.white;
    public float outlineProxWidth = 2.0f;

    [Header("Outline By Raycast")]
    public bool outlineByCast = true;
    public float outlineCastRange = 3.0f;
    public Color outlineCastColor = Color.yellow;
    public float outlineCastWidth = 3.0f;

    // Other
    private LayerMask nonHeldLayer;

    void Start()
    {
        nonHeldLayer = ~(1 << LayerMask.NameToLayer("holdLayer"));
        canvas = GameObject.FindWithTag("Canvas");
        scoreTrackerScript = canvas.GetComponentInChildren<Score>();
    }

    void Update()
    {
        if (Input.GetKeyDown(PlayerKeybinds.INTERACT_KEY))
        {
            Ray r = new Ray(InteractSource.position, InteractSource.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                    scoreTrackerScript.IncScore(15);


                }
            }
            
        }
    }

    private void FixedUpdate()
    {
        ClearActiveOutlines();

        if (outlineByProx)
        {
            AddOutlineByProximity();
        }

        if (outlineByCast)
        {
            AddOutlineByRaycast();
        }
    }

    #region Outlines
    private void ClearActiveOutlines()
    {
        GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (GameObject obj in interactableObjects)
        {
            if (obj.GetComponent<Outline>() != null)
            {
                obj.GetComponent<Outline>().enabled = false;
            }
        }
    }

    private void AddOutlineByProximity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(InteractSource.position, outlineProxRange, nonHeldLayer);
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

    private void AddOutlineByRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(InteractSource.position, InteractSource.forward, out hit, outlineCastRange, nonHeldLayer))
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

    #endregion
}
