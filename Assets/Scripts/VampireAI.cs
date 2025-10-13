using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
public class VampireAI : MonoBehaviour
{
    public float stoppingDistance = 1f;
    public float doorCenterOffset = 0.5f;
    public enum VampireState { FreeRoam, DoorCamp, Lure, Dialogue }
    public VampireState currentState;

    public float roamSpeed = 2f;
    [Header("NavMesh Tuning")]
    public float navAcceleration = 20f;
    public float navAngularSpeed = 720f;
    public bool navAutoBraking = false;
    public float stateDuration = 5f; // Time spent in each state before transitioning
    private float stateTimer;

    private Vector3 roamDirection;
    private float directionChangeInterval = 3f;
    private float directionChangeTimer;

    private Transform targetDoor;
    private Transform targetLureDoor;

    private bool wasInDialogue = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform[] campPoints;
    public string campTag = "camp";
    private int currentCampIndex = 0;

    void Start()
    {
        currentState = VampireState.FreeRoam;
        stateTimer = stateDuration;

        PickNewDirection();
        directionChangeTimer = directionChangeInterval;

        PickNewDoor();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Sync NavMeshAgent movement with our roam speed and tuning
        agent.speed = roamSpeed;
        agent.acceleration = navAcceleration;
        agent.angularSpeed = navAngularSpeed;
        agent.autoBraking = navAutoBraking;
        agent.stoppingDistance = stoppingDistance;

        // Find all camp points by tag
        GameObject[] campObjects = GameObject.FindGameObjectsWithTag(campTag);
        campPoints = new Transform[campObjects.Length];
        for (int i = 0; i < campObjects.Length; i++)
        {
            campPoints[i] = campObjects[i].transform;
        }

        if (campPoints.Length > 0)
        {
            currentCampIndex = Random.Range(0, campPoints.Length);
            agent.SetDestination(campPoints[currentCampIndex].position);
        }
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        directionChangeTimer -= Time.deltaTime;

        switch (currentState)
        {
            case VampireState.FreeRoam:
                FreeRoam();
                break;
            case VampireState.DoorCamp:
                DoorCamp();
                break;
            case VampireState.Lure:
                Lure();
                break;
            case VampireState.Dialogue:
                Dialogue();
                break;
        }

        if (currentState != VampireState.Dialogue && stateTimer <= 0f)
        {
            AdvanceState();
        }

        // After switch: check for dialogue end and resume
        if (currentState == VampireState.Dialogue && !DialogueManager.IsConversationActive && wasInDialogue)
        {
            wasInDialogue = false;
            AdvanceState();
        }

        if (animator != null && agent != null)
        {
            bool isWalking = agent.velocity.magnitude > 0.1f && !agent.pathPending;
            animator.SetBool("isWalking", isWalking);
        }
    }

    void FreeRoam() // Add pathfinding once environment is set up
    {
        if (campPoints == null || campPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, campPoints.Length);
            } while (nextIndex == currentCampIndex && campPoints.Length > 1);

            currentCampIndex = nextIndex;
            agent.SetDestination(campPoints[currentCampIndex].position);
        }
    }

    void PickNewDirection()
    {
        // Pick a random direction
        float angle = Random.Range(0f, 360f);
        roamDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

        // needs to update to rotate the vampire to face direction
        Quaternion lookRotation = Quaternion.LookRotation(roamDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.3f);
    }

    void DoorCamp()
    {
        if (targetDoor == null)
        {
            Debug.LogWarning("No targetDoor assigned for Vampire. Cannot lure.");
            return;
        }

        Vector3 targetPosition = new Vector3(targetDoor.position.x + doorCenterOffset, targetDoor.position.y, targetDoor.position.z);
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(targetPosition);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Vampire is now camping at door.");
            // play idle animation?
        }
    }

    void Lure()
    {
        GameObject[] lureDoors = GameObject.FindGameObjectsWithTag("lure");
        if (lureDoors.Length == 0)
        {
            Debug.LogWarning("No doors found with tag 'lure'.");
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestLureDoor = null;

        foreach (GameObject door in lureDoors)
        {
            float dist = Vector3.Distance(transform.position, door.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestLureDoor = door.transform;
            }
        }

        if (closestLureDoor == null)
        {
            Debug.LogWarning("No suitable lure door found.");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f); // adjust range as needed
        LureableNPC closestNPC = null;
        float closestDistanceNPC = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            LureableNPC npc = hitCollider.GetComponent<LureableNPC>();
            if (npc != null)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                if (distance < closestDistanceNPC)
                {
                    closestDistanceNPC = distance;
                    closestNPC = npc;
                }
            }
        }

        if (closestNPC != null)
        {
            if (closestLureDoor.CompareTag("lure"))
            {
                closestNPC.LureTo(closestLureDoor);
                Debug.Log($"{closestNPC.name} is being lured to the targetDoor.");
            }
            else
            {
                Debug.LogWarning("targetDoor is not tagged 'lure'.");
            }
        }

        Debug.Log("Vampire is attempting to lure a nearby NPC...");
    }

    void Dialogue()
    {
        if (!wasInDialogue)
        {
            wasInDialogue = true;
            int chance = Random.Range(0, 100);
            string selectedConversation = (chance < 80) ? "NPCVampireLureDialogue" : "GameOver";
            DialogueManager.StartConversation(selectedConversation);
        }
    }

    void AdvanceState() // Cycle through states
    {
        switch (currentState)
        {
            case VampireState.FreeRoam:
                currentState = VampireState.DoorCamp;
                break;
            case VampireState.DoorCamp:
                currentState = VampireState.Lure;
                break;
            case VampireState.Lure:
                currentState = VampireState.Dialogue;
                break;
            case VampireState.Dialogue:
                currentState = VampireState.FreeRoam;
                PickNewDoor();
                break;
        }

        // NavMeshAgent handling for state transitions
        if (agent != null)
        {
            if (currentState == VampireState.DoorCamp)
            {
                agent.ResetPath();
            }
            else if (currentState == VampireState.FreeRoam && campPoints != null && campPoints.Length > 0)
            {
                if (!agent.hasPath)
                {
                    agent.SetDestination(campPoints[currentCampIndex].position);
                }
            }
        }

        stateTimer = stateDuration;
    }

    void PickNewDoor()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("camp");

        if (doors.Length > 0)
        {
            targetDoor = doors[Random.Range(0, doors.Length)].transform;
        }
        else
        {
            Debug.LogWarning("No doors found with tag 'camp'.");
        }
    }

    public void ReflectDirection()
    {
        roamDirection = -roamDirection; // reverse movement
        Quaternion lookRotation = Quaternion.LookRotation(roamDirection);
        transform.rotation = lookRotation;

        roamDirection = Vector3.Reflect(roamDirection, Vector3.right);
        roamDirection.y = 0f; // Make sure Y is zero so you don't flip vertically
        roamDirection.Normalize();
    }

}
