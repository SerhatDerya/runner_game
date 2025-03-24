using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private int initialPoolSize = 30;
    [SerializeField] private int expandAmount = 10; // Pool tükendiğinde genişleme miktarı
    
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    private int activeCount = 0;

    private void Awake()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("Collectible prefab not assigned in CollectiblePool!");
            return;
        }

        InitializePool();
    }

    public void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddNewCollectibleToPool();
        }
        Debug.Log($"Collectible pool initialized with {initialPoolSize} items");
    }

    private void AddNewCollectibleToPool()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("Cannot add collectible - prefab reference is null!");
            return;
        }

        GameObject collectible = Instantiate(collectiblePrefab);
        collectible.SetActive(false);
        collectible.transform.SetParent(this.transform); // Organize hierarchy
        objectPool.Enqueue(collectible);
    }

    public GameObject GetCollectible()
    {
        if (objectPool.Count == 0)
        {
            Debug.LogWarning("Collectible pool exhausted, expanding...");
            ExpandPool();
        }

        GameObject collectible = objectPool.Dequeue();
        collectible.SetActive(true);
        activeCount++;
        
        Debug.Log($"Collectible taken from pool. Active: {activeCount}, Inactive: {objectPool.Count}");
        return collectible;
    }

    private void ExpandPool()
    {
        for (int i = 0; i < expandAmount; i++)
        {
            AddNewCollectibleToPool();
        }
        Debug.Log($"Pool expanded by {expandAmount}. New size: {objectPool.Count + activeCount}");
    }

    public void ReturnToPool(GameObject collectible)
    {
        if (collectible == null)
        {
            Debug.LogWarning("Attempted to return null collectible to pool!");
            return;
        }

        collectible.SetActive(false);
        objectPool.Enqueue(collectible);
        activeCount--;
        
        Debug.Log($"Collectible returned to pool. Active: {activeCount}, Inactive: {objectPool.Count}");
    }
}