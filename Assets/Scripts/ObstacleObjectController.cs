using UnityEngine;

public class ObstacleController : GameObjectController
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.GameOver();
        }
    }
}