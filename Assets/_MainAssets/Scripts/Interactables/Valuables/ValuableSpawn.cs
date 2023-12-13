using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ValuableSpawn : MonoBehaviour
{
    [SerializeField] private ValuableType valuableType = ValuableType.Money;
    [SerializeField] private bool activeStatus = false;

    private void Awake()
    {
        activeStatus = false;
    }

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
