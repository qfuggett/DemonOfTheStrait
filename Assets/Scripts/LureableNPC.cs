using UnityEngine;
using UnityEngine.AI;

public class LureableNPC : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 1.5f;

    private Transform target;
    private bool isLured;
    private Animator animator;
    private NavMeshAgent agent;
    private bool externalInteraction;
    private NpcAI npcAI;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        npcAI = GetComponent<NpcAI>();
    }

    // Called by VampireAI to lure the NPC toward a specific lure point
    public void LureTo(Transform lurePoint)
    {
        Debug.Log($"[{gameObject.name}] Lure point: {lurePoint.name} at position {lurePoint.position}");
        target = lurePoint;
        isLured = true;
        externalInteraction = true;

        if (npcAI != null)
        {
            npcAI.SuspendBehaviour(true);
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.position);
        }
    }

    private void Update()
    {
        if (!isLured || agent == null || target == null)
        {
            if (animator != null)
            {
                animator.SetBool("isLured", false);
            }
            return;
        }


        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log($"[{gameObject.name}] Reached lure point!");

            agent.isStopped = true;
            agent.ResetPath(); // Clear the path so velocity becomes zero

            isLured = false;

            // Face the vampire
            GameObject vampire = GameObject.FindWithTag("Vampire");
            if (vampire != null)
            {
                Vector3 lookDir = (vampire.transform.position - transform.position).normalized;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }
            }
            if (animator != null)
            {
                animator.SetBool("isLured", false);
            }
        }
        else
        {
            if (animator != null)
            {
                bool walking = agent.velocity.magnitude > 0.1f;
                animator.SetBool("isLured", walking);
            }
        }


    }
}