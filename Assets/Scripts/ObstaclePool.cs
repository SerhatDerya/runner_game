using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    //public static ObstaclePool Instance;
    
    [SerializeField] private GameObject obstacleFullPrefab;
    [SerializeField] private GameObject obstacleHalfPrefab;
    public int poolSize = 50;
    
    public Queue<GameObject> objectPool = new Queue<GameObject>();
    
    private void Awake()
    {
        InitializePool();
    }
    
    public void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacle;
            float randomValue = Random.Range(0f, 1f);

            if (randomValue <= 0.6f)
            {
                obstacle = Instantiate(obstacleFullPrefab);
            }
            else
            {
                obstacle = Instantiate(obstacleHalfPrefab);
            }

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