using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObstacleSpawner : BaseSpawner<MonoBehaviour, PoolManager>
{
    [SerializeField] private List<string> poolTags = new List<string> { "ObstacleHalf", "ObstacleFull" };
    private Dictionary<float, int> laneObstacleCounts = new();
    private Dictionary<float, int> zPosObstacleCounts = new();
    
    // For compatibility with existing code
    public void SpawnObstacles(GameObject platformObj)
    {
        SpawnObjects(platformObj);
    }
    
    // Override the abstract method from BaseSpawner
    public override void SpawnObjects(GameObject platformObj)
    {
        if (platformObj == null)
        {
            Debug.LogError("Platform object is null!");
            return;
        }

        Platform platform = platformObj.GetComponent<Platform>();
        if (platform == null)
        {
            Debug.LogError("Platform script is missing!");
            return;
        }

        List<Vector3> availablePoints = platform.GetAvailablePoints();

        if (availablePoints == null || availablePoints.Count == 0)
        {
            Debug.LogWarning("No available points on the platform for obstacle spawning!");
            return;
        }

        foreach (Vector3 spawnPoint in availablePoints.ToList())
        {
            if (Random.value <= spawnChance && !platform.IsPointOccupied(spawnPoint))
            {
                float laneX = spawnPoint.x;
                float laneZ = spawnPoint.z;

                // Aynı Z ekseninde çok fazla engel olup olmadığını kontrol et
                if (!zPosObstacleCounts.ContainsKey(laneZ))
                {
                    zPosObstacleCounts[laneZ] = 0;
                }
                else if (zPosObstacleCounts[laneZ] >= (laneManager.laneCount - 1))
                {
                    continue;
                }

                // Rastgele bir engel etiketi seç
                string selectedTag = poolTags[Random.Range(0, poolTags.Count)];
                GameObject obstacle = PoolManager.Instance.Get(selectedTag);

                if (obstacle != null)
                {
                    // Platformun üst yüzey yüksekliğini hesapla
                    Collider platformCollider = platformObj.GetComponent<Collider>();
                    Collider obstacleCollider = obstacle.GetComponent<Collider>();

                    if (platformCollider != null && obstacleCollider != null)
                    {
                        float platformTopY = platformCollider.bounds.max.y;
                        float obstacleBottomY = obstacleCollider.bounds.min.y;

                        // Engel pozisyonunu ayarla
                        float spawnHeight = platformTopY - obstacleBottomY;
                        obstacle.transform.position = new Vector3(spawnPoint.x, spawnHeight, spawnPoint.z);
                        obstacle.SetActive(true);
                        activeObjects.Enqueue(obstacle);

                        // Platformdaki bu noktayı işaretle
                        if (!platform.IsPointOccupied(spawnPoint))
                        {
                            // Spawn işlemi
                            platform.MarkPointOccupied(spawnPoint);
                        }
                        availablePoints.Remove(spawnPoint);

                        // Şerit ve Z ekseni sayacı güncelle
                        if (!laneObstacleCounts.ContainsKey(laneX))
                        {
                            laneObstacleCounts[laneX] = 0;
                        }
                        laneObstacleCounts[laneX]++;
                        zPosObstacleCounts[laneZ]++;
                    }
                }
            }
        }
    }

    // For compatibility with existing code
    public void ClearObstacles(GameObject platform)
    {
        ClearObjects(platform);
    }
    
    // Override base method
    public override void ClearObjects(GameObject platform)
    {
        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
        {
            platformScript.ClearOccupiedPoints();
        }

        float minZ = platform.transform.position.z - platform.transform.localScale.z / 2;
        float maxZ = platform.transform.position.z + platform.transform.localScale.z / 2;

        int count = activeObjects.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = activeObjects.Dequeue();
            if (obj.transform.position.z >= minZ && obj.transform.position.z <= maxZ)
            {
                obj.SetActive(false);
                ReturnObjectToPool(obj);
            }
            else
            {
                activeObjects.Enqueue(obj);
            }
        }
    }

    // For compatibility with existing code
    public void ClearAllObstacles()
    {
        ClearObjects();
    }
    
    // Need to implement this method for the abstract class
    protected override GameObject GetObjectFromPool()
    {
        // Choose a random obstacle tag if none is specified
        string selectedTag = poolTags[Random.Range(0, poolTags.Count)];
        return PoolManager.Instance.Get(selectedTag);
    }
    
    // Need to implement this method for the abstract class
    protected override void ReturnObjectToPool(GameObject obj)
    {
        PoolManager.Instance.Return(obj);
    }
}
