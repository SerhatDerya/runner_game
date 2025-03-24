using UnityEngine;

public class CollectibleController : GameObjectController
{    
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnDespawn();
        }
    }
}