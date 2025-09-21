using UnityEngine;

public class VampireBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vampire")) // Tag for Vampire
        {
            Debug.Log("Vampire hit boundary, reversing direction.");
            VampireAI vamp = other.GetComponent<VampireAI>();
            if (vamp != null)
            {
                vamp.ReflectDirection();
            }
        }
    }
}