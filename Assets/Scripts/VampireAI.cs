using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
public class VampireAI : MonoBehaviour
{
    public float stoppingDistance = 1f;
    public enum VampireState { FreeRoam, DoorCamp, Lure, Dialogue }
    public VampireState currentState;
    public float roamSpeed = 2f;
    [Header("NavMesh Tuning")]

    [Header("Lure Settings")]
    public float lureRadius = 100f;
    public float navAcceleration = 20f;
    public float navAngularSpeed = 720f;
    public bool navAutoBraking = false;
    public float stateDuration = 5f; // Time spent in each state before transitioning
    private float stateTimer;
    private Transform targetDoor;
    private Transform currentTargetWindow;
    private bool wasInDialogue = false;
    private bool hasLuredNPC = false;
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject[] windows;
    private GameObject[] camp;
    private GameObject[] lurePoints;
    private int lastWindowIndex = -1;
    private int lastCampIndex = -1;

    void Start()
    {
        // State Duration timer
        stateTimer = stateDuration;

        // Initialize navigation mesh agent and animator
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = roamSpeed;
            agent.acceleration = navAcceleration;
            agent.angularSpeed = navAngularSpeed;
            agent.autoBraking = navAutoBraking;
            agent.stoppingDistance = stoppingDistance;
            agent.ResetPath();
        }

        // Find all window points
        windows = GameObject.FindGameObjectsWithTag("window");
        camp = GameObject.FindGameObjectsWithTag("camp");
        lurePoints = GameObject.FindGameObjectsWithTag("lure");
        Debug.Log($"VampireAI initialized — {windows.Length} windows, {camp.Length} camps, {lurePoints.Length} lure points found.");

        // Start in FreeRoam state
        currentState = VampireState.FreeRoam;
    }

    void Update()
    {
        bool finishedState = false;

        switch (currentState)
        {
            case VampireState.FreeRoam:
                finishedState = FreeRoam();
                break;
            case VampireState.DoorCamp:
                finishedState = DoorCamp();
                break;
            case VampireState.Lure:
                finishedState = Lure();
                break;
            case VampireState.Dialogue:
                finishedState = Dialogue();
                break;
        }

        if (finishedState) // When each state returns true, advance to the next state
        {
            AdvanceState();
        }

        // Animation sync remains the same
        if (animator != null && agent != null)
        {
            bool isWalking = agent.velocity.magnitude > 0.1f && !agent.pathPending;
            animator.SetBool("isWalking", isWalking);
        }
    }

    bool FreeRoam()
    {

        if (windows == null || windows.Length == 0)
        {
            Debug.LogWarning("No window points found with tag 'window'.");
            return true; // Skip to next state if no windows
        }

        // Pick a random window only if we don't have one
        if (currentTargetWindow == null)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, windows.Length);
            }
            while (randomIndex == lastWindowIndex && windows.Length > 1);

            lastWindowIndex = randomIndex;
            currentTargetWindow = windows[randomIndex].transform;

            agent.SetDestination(currentTargetWindow.position);
            Debug.Log($"Vampire moving to new random window: {currentTargetWindow.name}");
        }
        // Check if we've reached the window
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Stay before transitioning
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                // reset for next free roam cycle
                currentTargetWindow = null;
                stateTimer = stateDuration;
                return true; // finished state, proceed to DoorCamp
            }
        }
        return false; // not yet finished state
    }

    bool DoorCamp()
    {
        // Ensure camp points exist
        if (camp == null || camp.Length == 0)
        {
            Debug.LogWarning("No camp points found in scene.");
            return true; // Skip to next state if no camp points
        }

        // Choose a random camp point if not already selected
        if (targetDoor == null)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, camp.Length);
            }
            while (randomIndex == lastCampIndex && camp.Length > 1);

            lastCampIndex = randomIndex;
            targetDoor = camp[randomIndex].transform;

            agent.SetDestination(targetDoor.position);
            Debug.Log($"Vampire moving to camp door: {targetDoor.name}");
        }

        // Wait until the vampire reaches the target
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                targetDoor = null; // reset for next cycle
                stateTimer = stateDuration;
                return true; // finished camping, move to next state
            }
        }

        return false; // still camping
    }

    bool Lure()
    {
        // Vampire stays stationary at its current camp point.

        // Ensure lure points exist
        if (lurePoints == null || lurePoints.Length == 0)
        {
            Debug.LogWarning("No lure points found in scene.");
            return true;
        }

        // Find the nearest lure door (destination for NPCs)
        Transform nearestLureDoor = null;
        float nearestLureDist = Mathf.Infinity;

        foreach (GameObject lureObj in lurePoints)
        {
            float dist = Vector3.Distance(transform.position, lureObj.transform.position);
            if (dist < nearestLureDist)
            {
                nearestLureDist = dist;
                nearestLureDoor = lureObj.transform;
            }
        }

        if (nearestLureDoor == null)
        {
            Debug.LogWarning("No valid lure door found.");
            return true;
        }

        // Make vampire face the lure door
        Vector3 directionToLureDoor = (nearestLureDoor.position - transform.position).normalized;
        directionToLureDoor.y = 0; // Keep rotation on horizontal plane only
        if (directionToLureDoor != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLureDoor);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Find the single closest LureableNPC by tag
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("npc");
        LureableNPC closestNPC = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject npcObj in allNPCs)
        {
            LureableNPC npc = npcObj.GetComponent<LureableNPC>();
            if (npc != null)
            {
                float dist = Vector3.Distance(transform.position, npcObj.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestNPC = npc;
                }
            }
        }

        // Lure the single closest NPC only once per state
        if (closestNPC != null && !hasLuredNPC)
        {
            Debug.Log($"[VampireAI] Found NPC {closestNPC.name} — calling LureTo()");
            hasLuredNPC = true;
            closestNPC.LureTo(nearestLureDoor);
            Debug.Log($"{closestNPC.name} is being lured to {nearestLureDoor.name}.");
        }

        // Vampire waits at camp for the state duration
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            // STOP the vampire BEFORE transitioning to Dialogue
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero; // Force velocity to zero
                agent.ResetPath();
            }

            stateTimer = stateDuration;
            hasLuredNPC = false;
            return true; // Finished luring, advance state
        }

        return false; // Still in lure state
    }

    bool Dialogue()
    {
        if (!wasInDialogue)
        {
            wasInDialogue = true;

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }

            int chance = Random.Range(0, 100);
            string selectedConversation = (chance < 80) ? "NPCVampireLureDialogue" : "GameOver";
            DialogueManager.StartConversation(selectedConversation);
            Debug.Log($"Starting conversation: {selectedConversation}");
        }
        // Check if dialogue has finished
        if (!DialogueManager.IsConversationActive)
        {
            wasInDialogue = false;
            stateTimer = stateDuration;

            // re-enable the agent before transitioning back to FreeRoam
            if (agent != null)
            {
                agent.ResetPath();           // Clear any old destination
                agent.velocity = Vector3.zero;
                agent.isStopped = false;
            }
            return true; // Done, move on to free roam
        }
        return false; // Wait until dialogue is done

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
                break;
        }
        // Reset timer per state
        stateTimer = stateDuration;
    }

}
