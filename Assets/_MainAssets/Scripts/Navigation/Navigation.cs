using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// Controller for AI Navigation
public class Navigation : MonoBehaviour
{
    public Transform player;
    public float max_dist = 15f;
    private NavMeshAgent agent;

    private float targetDist;
    public float distractionRange = 30f; // Range at which AI gets distracted by thrown object
    public float distractionTime = 5f; // Time AI is distracted before resuming previous behavior

    public bool isDistracted = false;
    private Vector3 distractionPosition;
    private float distractionTimer;

    private GameObject[] patrolPoints;

    private Door[] doors;
    public float doorOpenRange = 2.0f;
    public bool isActive = false;

    private EscapeController escapeController;

    // Start is called before the first frame update
    void Start()
    {        
        agent = GetComponent<NavMeshAgent>();
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");

        // get all doors
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        List<Door> doorList = new List<Door>();
        foreach (GameObject interactable in interactables)
        {
            if (interactable.GetComponent<Door>() != null)
            {
                doorList.Add(interactable.GetComponent<Door>());
            }
        }
        doors = doorList.ToArray();
    
        escapeController = GameObject.FindGameObjectWithTag("Escape").GetComponent<EscapeController>();
        // TimerManager.Instance.StartTimer(escapeController.GetMissionDuration(), null, this.ActivateAI);

    }

    void Update() 
    {
        if (isActive)
        {
            targetDist = Vector3.Distance(player.position, transform.position);

            CheckPlayerCollision();
            OpenNearbyDoors();

            if (!isDistracted && targetDist < 15 && HasLineOfSightToPlayer())
            {
                agent.SetDestination(player.position);
            }

            else if (isDistracted && distractionTimer > 0f)
            {
                // AI is distracted, head towards the distraction position
                agent.SetDestination(distractionPosition);
                distractionTimer -= Time.deltaTime;

                // Reset distraction
                if (distractionTimer <= 0f) isDistracted = false;
            }

            else if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                // If the AI can't find the player and has reached the last known position, go to a random patrol point
                if (patrolPoints.Length > 0)
                {
                    int randomIndex = Random.Range(0, patrolPoints.Length);
                    GameObject randomPatrolPoint = patrolPoints[randomIndex];

                    // Set the destination to the selected patrol point
                    agent.SetDestination(randomPatrolPoint.transform.position);
                }
            }

            if (targetDist <= agent.stoppingDistance)
            {
                // animator.SetFloat("forward", 2.0f);
                // FaceTarget();
            }

        }



    }

    public void ActivateAI()
    {
        isActive = true;
    }

    private void OpenNearbyDoors()
    {
        foreach(Door door in doors)
        {
            if (Vector3.Distance(transform.position, door.gameObject.transform.position) < doorOpenRange)
            {
                door.DoorOpenOverride(this);
            }
        }
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;

        // Cast a ray from the AI towards the player
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, max_dist))
        {
            // Check if the hit object is the player (assuming the player has a tag "Player")
            if (hit.collider.CompareTag("Player"))
            {
                // Line of sight to the player is not obstructed
                return true;
            }
        }
        // Line of sight is obstructed
        return false;
    }


    private bool CheckPlayerCollision()
    {
        bool caughtPlayer = false;

        if(Vector3.Distance(transform.position, player.transform.position) < 1.0f)
        {
            caughtPlayer = true;
            escapeController.EscapeFailed();
            isActive = false;
        }

        

        return caughtPlayer;
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Called when a throwable object is thrown and lands within distraction range
    public void Distract(Vector3 position)
    {
        if (!isDistracted && !HasLineOfSightToPlayer())
        {
            // Set distraction position and start distraction timer
            isDistracted = true;
            distractionPosition = position;
            distractionTimer = distractionTime;
        }
    }

}
