using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable checkpoint once drone touches it
            gameObject.SetActive(false);
        }
    }
}
