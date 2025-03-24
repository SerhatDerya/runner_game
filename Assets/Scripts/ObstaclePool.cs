using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    [SerializeField] private GameObject obstacleFullPrefab;
    [SerializeField] private GameObject obstacleHalfPrefab;
    [SerializeField] private int initialPoolSize = 50;
    [SerializeField] private int expandAmount = 10;
    
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    private int activeCount = 0;

    private void Awake()
    {
        InitializePool();
    }

    public void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddNewObstacleToPool();
        }
    }

    private void AddNewObstacleToPool()
    {
        GameObject obstacle = Random.Range(0f, 1f) <= 0.6f ? 
            Instantiate(obstacleFullPrefab) : 
            Instantiate(obstacleHalfPrefab);
        
        obstacle.SetActive(false);
        objectPool.Enqueue(obstacle);
    }

    public GameObject GetObstacle()
    {
        if (objectPool.Count == 0)
        {
            Debug.LogWarning("Pool empty, expanding...");
            ExpandPool();
        }

        GameObject obstacle = objectPool.Dequeue();
        obstacle.SetActive(true);
        activeCount++;
        return obstacle;
    }

    private void ExpandPool()
    {
        for (int i = 0; i < expandAmount; i++)
        {
            AddNewObstacleToPool();
        }
        Debug.Log($"Pool expanded. New size: {objectPool.Count + activeCount}");
    }

    public void ReturnToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
        objectPool.Enqueue(obstacle);
        activeCount--;
    }
}