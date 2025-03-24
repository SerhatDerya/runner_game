using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : BaseSpawner<ObstacleController, ObstaclePool>
{
    [SerializeField] private float fullObstacleSpawnChance = 0.6f;
    
    public override void SpawnObjects(GameObject platform)
{
    // Null kontrolleri ekleyin
    if (platform == null || laneManager == null || objectPool == null)
    {
        Debug.LogError("Critical references missing!");
        return;
    }

    float spawnY = CalculateSpawnHeight(platform);
    
    float spawnAreaMinZ, spawnAreaMaxZ;
    CalculateSpawnAreaBounds(platform, out spawnAreaMinZ, out spawnAreaMaxZ);

    int laneCount = laneManager.laneCount;
    int milestoneCount = Mathf.FloorToInt(platform.transform.localScale.z / milestoneInterval);

    for (int i = 1; i <= milestoneCount; i++)
    {
        int obstacleFullCountOfMilestone = 0;
        for (int j = 0; j < laneCount; j++)
        {
            float randomChanceValue = Random.Range(0f, 1f);
            float spawnZ = spawnAreaMinZ + i * milestoneInterval;
            
            if (obstacleFullCountOfMilestone != laneCount-1)
            {
                if (randomChanceValue <= spawnChance)
                {
                    float spawnX = laneManager.GetLanePosition(j);
                    Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

                    GameObject obstacle = GetObjectFromPool();
                    
                    // Null kontrolü ekleyin
                    if (obstacle != null)
                    {
                        obstacle.transform.position = spawnPosition;
                        activeObjects.Enqueue(obstacle);
                        
                        if(obstacle.CompareTag("ObstacleFull"))
                        {
                            obstacleFullCountOfMilestone++;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to get obstacle from pool!");
                    }
                }
            }
            else
            {
                break;
            }
        }
    }
}
    
    protected override GameObject GetObjectFromPool()
    {
        return objectPool.GetObstacle();
    }
    
    protected override void ReturnObjectToPool(GameObject obj)
    {
        objectPool.ReturnToPool(obj);
    }
}