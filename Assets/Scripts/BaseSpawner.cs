using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawner<T, P> : MonoBehaviour where T : MonoBehaviour where P : MonoBehaviour
{
    [SerializeField] protected P objectPool;
    [SerializeField] protected LaneManager laneManager;

    protected Queue<GameObject> activeObjects = new Queue<GameObject>();
    
    [SerializeField] protected float milestoneInterval = 30f;
    [SerializeField] protected float spawnChance = 0.75f;
    
   
    public abstract void SpawnObjects(GameObject platform);
    
  
    public virtual void ClearObjects(GameObject platform)
    {
        float platformMinZ = platform.transform.position.z - platform.transform.localScale.z/2;
        float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z/2;
        
        for(int i=0; i<activeObjects.Count; i++)
        {   
            GameObject peekObject = activeObjects.Peek();
            if(peekObject.transform.position.z >= platformMinZ && peekObject.transform.position.z <= platformMaxZ)
            {
                ReturnObjectToPool(activeObjects.Dequeue());
            }
            else
            {
                break;
            }
        }
    }
    
    
    public virtual void ChangeObjectPositions(GameObject platform)
    {
        // Önce platformdaki nesneleri temizle
        ClearObjects(platform);
        
        // Sonra platform için yeni nesneler oluştur
        SpawnObjects(platform);
    }
    
    
    public virtual void ClearObjects()
    {
        while(activeObjects.Count > 0)
        {
            ReturnObjectToPool(activeObjects.Dequeue());
        }
    }
    
    
    protected abstract void ReturnObjectToPool(GameObject obj);
    
    
    protected abstract GameObject GetObjectFromPool();
    

    protected float CalculateSpawnHeight(GameObject platform, float objectHeight = 0.5f)
    {
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;
        return platform.transform.position.y + platformHeight + objectHeight;
    }
    
    
    protected void CalculateSpawnAreaBounds(GameObject platform, out float minZ, out float maxZ)
    {
        minZ = platform.transform.position.z - platform.transform.localScale.z/2;
        maxZ = platform.transform.position.z + platform.transform.localScale.z/2;
    }
}