using UnityEngine;

public class LureableNPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private Transform target;
    private bool isLured = false;

    public void LureTo(Transform lurePoint)
    {
        target = lurePoint;
        isLured = true;
    }

    private void Update()
    {
        if (isLured && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance >= stoppingDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
                isLured = false;
                Debug.Log($"{gameObject.name} has reached stopping distance from the target.");
            }
        }
    }
}
