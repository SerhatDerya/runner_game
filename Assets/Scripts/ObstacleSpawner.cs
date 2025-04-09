using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private List<string> poolTags = new List<string> { "ObstacleHalf", "ObstacleFull" };
    [SerializeField] private LaneManager laneManager;
    [SerializeField] private float spawnChance = 0.75f;

    private Queue<GameObject> activeObstacles = new();
    private Dictionary<float, int> laneObstacleCounts = new();
    private Dictionary<float, int> zPosObstacleCounts = new();
    public void SpawnObstacles(GameObject platformObj)
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
                        activeObstacles.Enqueue(obstacle);

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

    public void ClearObstacles(GameObject platform)
    {
        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
        {
            platformScript.ClearOccupiedPoints();
        }

        float minZ = platform.transform.position.z - platform.transform.localScale.z / 2;
        float maxZ = platform.transform.position.z + platform.transform.localScale.z / 2;

        int count = activeObstacles.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = activeObstacles.Dequeue();
            if (obj.transform.position.z >= minZ && obj.transform.position.z <= maxZ)
            {
                obj.SetActive(false);
                PoolManager.Instance.Return(obj);
            }
            else
            {
                activeObstacles.Enqueue(obj);
            }
        }
    }

    public void ClearAllObstacles()
    {
        while (activeObstacles.Count > 0)
        {
            GameObject obj = activeObstacles.Dequeue();
            obj.SetActive(false);
            PoolManager.Instance.Return(obj);
        }
    }
}
