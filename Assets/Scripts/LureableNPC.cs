using UnityEngine;

/// Handles switching an NPC into an externally-controlled lure state and resuming the
/// ambient wander behaviour once the interaction completes.
public class LureableNPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private Transform target;
    private bool isLured;
    private bool externalInteraction;
    private NpcAI npcAI;

    private void Awake()
    {
        npcAI = GetComponent<NpcAI>();
    }

    public void LureTo(Transform lurePoint)
    {
        target = lurePoint;
        isLured = true;
        externalInteraction = true;

        if (npcAI != null)
        {
            npcAI.SuspendBehaviour(true);
        }
    }

    /// Call once the external interaction (for example the dialogue) is done so
    /// the NPC can fall back to its ambient behaviour.
    public void EndExternalInteraction()
    {
        externalInteraction = false;

        if (npcAI != null)
        {
            npcAI.ResumeAfterExternalControl();
        }
    }

    private void Update()
    {
        if (!isLured || target == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > stoppingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            isLured = false;
            // Interaction continues until EndExternalInteraction is called.
        }
    }

    public bool IsInExternalInteraction => externalInteraction;
}
