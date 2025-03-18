using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    //public static ObstaclePool Instance;
    
    [SerializeField] private GameObject obstaclePrefab;
    public int poolSize = 30;
    
    public Queue<GameObject> objectPool = new Queue<GameObject>();
    
    private void Awake()
    {
        InitializePool();
    }
    
    public void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefab);
            obstacle.SetActive(false);
            objectPool.Enqueue(obstacle);
        }
    }
    
    public GameObject GetObstacle()
    {
        if (objectPool.Count > 0)
        {
            GameObject obstacle = objectPool.Dequeue();
            obstacle.SetActive(true);
            return obstacle;
        }
        else
        {
            return null;
        }
    }
    
    public void ReturnToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
        objectPool.Enqueue(obstacle);
    }
}