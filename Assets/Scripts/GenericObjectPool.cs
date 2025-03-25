using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> : MonoBehaviour where T : Component, IPoolable
{
    [Header("Pool Settings")]
    [SerializeField] protected T prefab;
    [SerializeField] private int initialPoolSize = 50;
    [SerializeField] private int expandAmount = 10;

    protected Queue<T> objectPool = new Queue<T>();
    private int activeCount = 0;

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab not assigned in GenericObjectPool!");
            return;
        }

        InitializePool();
    }

    public void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddNewObjectToPool();
        }
        //Debug.Log($"Pool initialized with {initialPoolSize} items");
    }

    public virtual void AddNewObjectToPool()
    {
        
        T obj = Instantiate(prefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.transform); // Organize hierarchy
        objectPool.Enqueue(obj);
    }

    public T GetObject()
    {
        if (objectPool.Count == 0)
        {
            //Debug.Log("Pool empty, expanding...");
            ExpandPool();
        }

        T obj = objectPool.Dequeue();
        obj.gameObject.SetActive(true);
        activeCount++;
        
        //Debug.Log($"Object taken from pool. Active: {activeCount}, Inactive: {objectPool.Count}");
        return obj;
    }

    private void ExpandPool()
    {
        for (int i = 0; i < expandAmount; i++)
        {
            AddNewObjectToPool();
        }
        //Debug.Log($"Pool expanded. New size: {objectPool.Count + activeCount}");
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj);
        activeCount--;
        
        //Debug.Log($"Object returned to pool. Active: {activeCount}, Inactive: {objectPool.Count}");
    }
}