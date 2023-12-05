using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignDefaultPhysicMaterial : MonoBehaviour
{
    public bool shouldAssignDefaultMaterialToAllNulls = false;
    public PhysicMaterial defaultMaterial;

    private void Start()
    {
        Collider[] colliders = GameObject.FindObjectsOfType<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.sharedMaterial == null)
            {
                collider.sharedMaterial = defaultMaterial;
            }
        }
    }
}
