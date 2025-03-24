using UnityEngine;

public abstract class GameObjectController : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;
    
    public virtual void OnSpawn() 
    {
        gameObject.SetActive(true);
    }
    
    public virtual void OnDespawn() 
    {
        gameObject.SetActive(false);
    }
    
    protected virtual void OnTriggerEnter(Collider other) { }
}