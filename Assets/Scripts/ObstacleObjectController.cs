using UnityEngine;

public class ObstacleObjectController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Player ile temas
        {
            GameManager.instance.GameOver();
        }
    }
}