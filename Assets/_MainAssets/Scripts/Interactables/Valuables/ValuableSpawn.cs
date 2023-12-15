using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Possible spawn location of a valuable. 
public class ValuableSpawn : MonoBehaviour
{
    [SerializeField] private ValuableType valuableType = ValuableType.Money;
    [SerializeField] private bool activeStatus = false; // whether or not the spawn is/was used

    private void Awake()
    {
        activeStatus = false;
    }

    // Instantiate a valuable prefab at this spawn if it is of the same ValuableType.
    public void MakeActive(GameObject valuableObject)
    {
        if (valuableObject.GetComponent<ValuableItem>() != null && valuableObject.GetComponent<ValuableItem>().GetValuableType() == valuableType)
        {
            activeStatus = true;
            Instantiate(valuableObject, transform);
        }
    }

    public bool GetActiveStatus()
    {
        return activeStatus;
    }

    public ValuableType GetValuableType()
    {
        return valuableType;
    }

}
