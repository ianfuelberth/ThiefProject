using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

public class ValuableSpawnManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int CHECKLIST_COUNT = 5;
    [SerializeField] private int EXTRA_VALUABLES_COUNT = 5;
    [SerializeField] private int MONEY_SPAWN_COUNT = 5;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] moneyPrefabs;
    [SerializeField] private GameObject[] trophyPrefabs;
    [SerializeField] private GameObject[] figureinePrefabs;
    [SerializeField] private GameObject[] cellphonePrefabs;
    [SerializeField] private GameObject[] briefcasePrefabs;
    [SerializeField] private GameObject[] vhsPrefabs;
    [SerializeField] private GameObject[] floppydiskPrefabs;
    
    // Helper to keep track of current list of specific prefab type
    private List<GameObject> prefabList;

    [Header("Spawns")]
    [SerializeField] private List<ValuableSpawn> checklistSpawns;
    [SerializeField] private List<GameObject> activeSpawns;
    private List<ValuableSpawn> remainingSpawnLocations;
    private List<ValuableSpawn> remainingMoneySpawns;
    
    
    [Header("References")]
    [SerializeField] private ChecklistController checklistController;



    void Start()
    {
        InitializeSpawnLocations();

        SpawnChecklistValuables();

        SpawnExtraValuables();

        SpawnExtraMoney();
    }

    private void SpawnChecklistValuables()
    {
        List<ValuableType> checklistItems = new List<ValuableType>();

        Random rnd = new Random();
        int currentCount = 0;
        int attemptsLeft = 10; // prevent possible infinite. shouldn't happen if there are enough valid spawns in scene.
        
        while (currentCount < CHECKLIST_COUNT && attemptsLeft > 0)
        {
            int index = rnd.Next(remainingSpawnLocations.Count);
           
            ValuableSpawn newActiveSpawn = SpawnNewValuable(index);
                
            if (newActiveSpawn != null)
            {
                checklistSpawns.Add(newActiveSpawn);
                checklistItems.Add(newActiveSpawn.GetValuableType());
                currentCount++;
                attemptsLeft++;
            }

            attemptsLeft--;
        }

        checklistController.InitializeChecklist(checklistItems.ToArray());
    }

    private void SpawnExtraValuables()
    {
        Random rnd = new Random();
        int currentCount = 0;
        int attemptsLeft = 10; // prevent possible infinite. shouldn't happen if there are enough valid spawns in scene.

        while (currentCount < EXTRA_VALUABLES_COUNT && attemptsLeft > 0)
        {
            int index = rnd.Next(remainingSpawnLocations.Count);

            ValuableSpawn newActiveSpawn = SpawnNewValuable(index);

            if (newActiveSpawn != null)
            {
                currentCount++;
                attemptsLeft++;
            }

            attemptsLeft--;
        }
    }

    private void SpawnExtraMoney()
    {
        Random rnd = new Random();
        int currentCount = 0;
        int attemptsLeft = 10; // prevent possible infinite. shouldn't happen if there are enough valid spawns in scene.

        while (currentCount < MONEY_SPAWN_COUNT && attemptsLeft > 0)
        {
            int index = rnd.Next(remainingMoneySpawns.Count);

            ValuableSpawn newActiveSpawn = SpawnNewMoney(index);

            if (newActiveSpawn != null)
            {
                currentCount++;
                attemptsLeft++;
            }

            attemptsLeft--;
        }
    }

    private ValuableSpawn SpawnNewValuable(int spawnIndex)
    {
        ValuableSpawn removedSpawn = null;

        if (TryFillSpawnLocation(remainingSpawnLocations[spawnIndex]))
        {
            removedSpawn = remainingSpawnLocations[spawnIndex];
            activeSpawns.Add(remainingSpawnLocations[spawnIndex].gameObject);
            remainingSpawnLocations.RemoveAt(spawnIndex);
        }

        return removedSpawn;
    }

    private ValuableSpawn SpawnNewMoney(int spawnIndex)
    {
        ValuableSpawn removedSpawn = null;

        if (TryFillSpawnLocation(remainingMoneySpawns[spawnIndex]))
        {
            removedSpawn = remainingMoneySpawns[spawnIndex];
            activeSpawns.Add(remainingMoneySpawns[spawnIndex].gameObject);
            remainingMoneySpawns.RemoveAt(spawnIndex);
        }

        return removedSpawn;
    }

    private bool TryFillSpawnLocation(ValuableSpawn spawnLocation)
    {
        bool valid = false;

        ValuableType spawnType = spawnLocation.GetValuableType();
        GameObject valuablePrefab = GetPrefabOfType(spawnType);
        if (valuablePrefab != null)
        {
            valid = true;
            spawnLocation.MakeActive(valuablePrefab);
        }

        return valid;
        
    }

    private GameObject GetPrefabOfType(ValuableType type)
    {
        GameObject valuablePrefab = null;

        // List<GameObject> prefabs = new List<GameObject>();
        Random rnd = new Random();

        // Determine Prefab Type
        if (type == ValuableType.Money)
        {
            prefabList = moneyPrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.Trophy)
        {
            prefabList = trophyPrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.Figurine)
        {
            prefabList = figureinePrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.Cellphone)
        {
            prefabList = cellphonePrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.Briefcase)
        {
            prefabList = briefcasePrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.VhsTape)
        {
            prefabList = vhsPrefabs.ToList<GameObject>();
        }
        else if (type == ValuableType.FloppyDisk)
        {
            prefabList = floppydiskPrefabs.ToList<GameObject>();
        }
        else
        {
            return null;
        }

        // Get Prefab
        if (prefabList.Count > 0)
        {
            int index = 0;

            if (prefabList.Count > 1)
            {
                index = rnd.Next(prefabList.Count);
            }

            valuablePrefab = prefabList[index];

        }

        return valuablePrefab;
    }    

    private void InitializeSpawnLocations()
    {
        remainingSpawnLocations = new List<ValuableSpawn>();

        List<GameObject> spawnObjects = GameObject.FindGameObjectsWithTag("ValuableSpawn").ToList<GameObject>();
        foreach (GameObject go in spawnObjects)
        {
            ValuableSpawn spawn = go.GetComponent<ValuableSpawn>();
            if (spawn != null)
            {
                if (spawn.GetValuableType() == ValuableType.Money)
                {
                    remainingMoneySpawns.Add(spawn);
                }
                else
                {
                    remainingSpawnLocations.Add(spawn);
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
