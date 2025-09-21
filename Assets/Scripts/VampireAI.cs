using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VampireAI : MonoBehaviour
{
    public enum VampireState { FreeRoam, DoorCamp, Lure, Dialogue }
    public VampireState currentState;

    public float roamSpeed = 2f;
    public float stateDuration = 5f; // Time spent in each state before transitioning
    private float stateTimer;

    private Vector3 roamDirection;
    private float directionChangeInterval = 3f;
    private float directionChangeTimer;

    private Transform targetDoor;
    private Transform targetLureDoor;

    void Start()
    {
        currentState = VampireState.FreeRoam;
        stateTimer = stateDuration;

        PickNewDirection();
        directionChangeTimer = directionChangeInterval;

        PickNewDoor();
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
        }

        if (stateTimer <= 0f)
        {
            AdvanceState();
        }
    }

    void FreeRoam() // Add pathfinding once environment is set up
    {
        transform.Translate(roamDirection * roamSpeed * Time.deltaTime, Space.World);

        if (directionChangeTimer <= 0f)
        {
            PickNewDirection();
            directionChangeTimer = directionChangeInterval;
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

        Vector3 targetPosition = targetDoor.position;

        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, roamSpeed * Time.deltaTime);
        }
        else
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
                currentState = VampireState.FreeRoam;
                PickNewDoor();
                break;
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
