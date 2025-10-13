using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// Handles the ambient wandering behaviour of NPCs.
/// NPCs walk between randomly selected idle spots, pause there, and repeat.
/// This state suspends automatically when the LureableNPC takes over.
public class NpcAI : MonoBehaviour
{
    // Track occupied idle spots to avoid multiple NPCs crowding the same location
    private static HashSet<Transform> occupiedSpots = new HashSet<Transform>();
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitDuration = 3f;
    [SerializeField] private string idleTag = "idleSpot";
    [SerializeField, Range(0, 99)]

    private NavMeshAgent agent;
    private Animator animator;
    private Transform[] idleSpots;
    private int currentSpotIndex = -1;
    private bool isSuspended;
    private Coroutine idleRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (animator != null && agent != null)
        {
            bool isWalking = !agent.pathPending && agent.velocity.magnitude > 0.1f;
            animator.SetBool("isWalking", isWalking);
        }
    }

    private void Start()
    {
        agent.speed = moveSpeed;
        // Crowd control so NPCs don't bunch up
        agent.avoidancePriority = Random.Range(20, 80);

        // Find all idle spots tagged in the scene
        GameObject[] spots = GameObject.FindGameObjectsWithTag(idleTag);
        idleSpots = new Transform[spots.Length];
        for (int i = 0; i < spots.Length; i++)
        {
            idleSpots[i] = spots[i].transform;
        }

        if (idleSpots.Length > 0)
        {
            idleRoutine = StartCoroutine(IdleCycle());
        }
        else
        {
            Debug.LogWarning($"{name}: No idle spots found with tag '{idleTag}'.");
        }
    }

    private IEnumerator IdleCycle()
    {
        while (true)
        {
            if (isSuspended)
            {
                yield return null;
                continue;
            }

            // Pick a new random idle spot (avoid occupied)
            Transform nextSpot = null;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int randomIndex = Random.Range(0, idleSpots.Length);
                Transform candidate = idleSpots[randomIndex];
                if (!occupiedSpots.Contains(candidate))
                {
                    nextSpot = candidate;
                    occupiedSpots.Add(candidate);
                    break;
                }
            }

            if (nextSpot == null)
            {
                // fallback if all are taken
                nextSpot = idleSpots[Random.Range(0, idleSpots.Length)];
            }

            currentSpotIndex = System.Array.IndexOf(idleSpots, nextSpot);
            agent.SetDestination(nextSpot.position);
            Debug.Log($"{name} moving to idle spot: {nextSpot.name} at {nextSpot.position}");

            // Wait until NPC arrives
            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Wait at spot
            yield return new WaitForSeconds(waitDuration);

            // Release occupied spot only AFTER done waiting
            if (currentSpotIndex >= 0 && currentSpotIndex < idleSpots.Length)
            {
                occupiedSpots.Remove(idleSpots[currentSpotIndex]);
            }

            // Small random delay so all NPCs don't re-select at the same frame
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    /// Called by LureableNPC when external control begins
    public void SuspendBehaviour(bool suspend)
    {
        isSuspended = suspend;
        if (suspend)
        {
            agent.ResetPath();
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    /// Called when external control (lure or dialogue) ends
    public void ResumeAfterExternalControl()
    {
        isSuspended = false;
        if (idleRoutine == null)
        {
            idleRoutine = StartCoroutine(IdleCycle());
        }
    }
}