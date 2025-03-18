using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool obstaclePool;

    private Queue<GameObject> activeObstacles = new Queue<GameObject>();
    
    public void SpawnObstacles(GameObject platform)
    {
        int obstacleCount = Mathf.RoundToInt(obstaclePool.poolSize / 2);
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        float spawnAreaMinZ = platform.transform.position.z - 100f;
        float spawnAreaMaxZ = platform.transform.position.z + 100f;

        for (int i = 0; i < obstacleCount; i++)
        {
            float randomZ = Random.Range(spawnAreaMinZ, spawnAreaMaxZ);
            float randomY = platform.transform.position.y + platformHeight + 0.5f;
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            // Havuzdan engel al ve pozisyonunu ayarla
            GameObject obstacle = obstaclePool.GetObstacle();
            obstacle.transform.position = spawnPosition;
            activeObstacles.Enqueue(obstacle);
        }
    }

    public void ClearObstacles()
    {
        int halfCount = activeObstacles.Count / 2;
        
        // Return first half of obstacles to the pool
        for (int i = 0; i < halfCount; i++)
        {
            obstaclePool.ReturnToPool(activeObstacles.Dequeue());
        }
        
    }
    
    public void ChangeObstaclePosition(GameObject platform)
    {
        // Önce tüm engellerin yarısını havuza geri gönder
        ClearObstacles();
        
        // Sonra platform için yeni engeller oluştur
        SpawnObstacles(platform);
    }
}