using UnityEngine;

public class CollectableObjectController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Player ile temas
        {
            Destroy(gameObject);
        }
    }
}
