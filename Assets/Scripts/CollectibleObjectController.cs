using UnityEngine;

public class CollectibleObjectController : MonoBehaviour
{
    public CollectibleSpawner collectibleSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Player ile temas
        {
            gameObject.SetActive(false); // Collectible'ı görünmez yap
        }
    }
}
