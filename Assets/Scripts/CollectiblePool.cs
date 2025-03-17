using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool : MonoBehaviour
{
    public static CollectiblePool Instance;
    
    [SerializeField] private GameObject collectiblePrefab;
    public int poolSize = 30;
    
    public Queue<GameObject> objectPool = new Queue<GameObject>();
    
    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }   
        else{
            Destroy(gameObject);
        }
        
        InitializePool();
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject collectible = Instantiate(collectiblePrefab);
            collectible.SetActive(false);
            objectPool.Enqueue(collectible);
        }
    }
    
    public GameObject GetCollectible()
    {
        if (objectPool.Count > 0)
        {
            GameObject collectible = objectPool.Dequeue();
            collectible.SetActive(true);
            return collectible;
        }
        else
        {
            return null;
        }
    }
    
    public void ReturnToPool(GameObject collectible)
    {
        collectible.SetActive(false);
        objectPool.Enqueue(collectible);
        //Debug.Log(objectPool.Count);
    }
}
