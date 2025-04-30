using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class CollectibleSpawner : BaseSpawner<MonoBehaviour, PoolManager>
{
    [SerializeField] private string poolTag = "Collectible"; // Havuz etiketi
    [SerializeField] private int formationCount = 10;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private int coinsPerFormation = 5;
    [SerializeField] private float arcHeight = 2f;
    
    // Existing method with the same name for compatibility
    public void SpawnCollectibles(GameObject platformObj)
    {
        SpawnObjects(platformObj);
    }
    
    // Implementation of abstract method from BaseSpawner
    public override void SpawnObjects(GameObject platformObj)
    {
        Platform platform = platformObj.GetComponent<Platform>();
        if (platform == null)
        {
            Debug.LogError("Platform script is missing!");
            return;
        }

        List<Vector3> availablePoints = platform.GetAvailablePoints();

        for (int i = 0; i < formationCount; i++)
        {
            if (availablePoints.Count == 0) break;

            // Bir sonraki formasyonun şeridini belirle
            int laneIndex = platform.GetNextLaneIndex();
            float laneX = laneManager.GetLanePosition(laneIndex);

            // Şeritteki uygun spawn noktalarını al
            List<Vector3> lanePoints = availablePoints.Where(p => Mathf.Approximately(p.x, laneX)).ToList();
            if (lanePoints.Count == 0) continue;

            // Rastgele bir spawn noktası seç
            Vector3 spawnPoint = lanePoints[Random.Range(0, lanePoints.Count)];
            availablePoints.Remove(spawnPoint);

            // Obstacle kontrolü
            if (platform.IsObstacleInLane(laneIndex, spawnPoint.z))
            {
                continue;
            }

            List<Vector3> positions = CoinPattern.GetLinePattern(
                spawnPoint,
                coinsPerFormation,
                spawnDistance
            );

            // Platformda işgal edilen noktaları kontrol et
            if (!positions.Any(p => platform.IsPointOccupied(p)))
            {
                SpawnFormation(positions, platform);

                // İşaretleme
                foreach (Vector3 pos in positions)
                {
                    if (!platform.IsPointOccupied(pos))
                    {
                        // Spawn işlemi
                        platform.MarkPointOccupied(pos);
                    }
                }
            }
        }

        SpawnOverObstacles(platformObj, platform);
    }

    private void SpawnOverObstacles(GameObject platformObj, Platform platform)
    {
        GameObject[] halfObstacles = GameObject.FindGameObjectsWithTag("ObstacleHalf");

        // Find obstacles within range of the platform's z position
        float platformZ = platform.transform.position.z;
        float minZ = platformZ - 100f;
        float maxZ = platformZ + 100f;

        List<GameObject> obstacles = new List<GameObject>();
        foreach (GameObject halfObstacle in halfObstacles)
        {
            float obstacleZ = halfObstacle.transform.position.z;
            if (obstacleZ >= minZ && obstacleZ <= maxZ)
            {
                obstacles.Add(halfObstacle);
            }
        }

        foreach (GameObject obstacle in obstacles)
        {
            if (Random.value > 0.5f)
            {
                Collider obstacleCollider = obstacle.GetComponent<Collider>();
                if (obstacleCollider == null) continue;

                // Obstacle'ın üst noktasını hesapla
                float obstacleTopY = obstacleCollider.bounds.max.y;
                Vector3 obstaclePos = obstacle.transform.position;

                // Spawn pozisyonlarını hesapla
                List<Vector3> spawnPositions = CoinPattern.GetJumpArcPattern(
                    new Vector3(obstaclePos.x, obstacleTopY, obstaclePos.z), // Yüksekliği obstacle'ın üstüne ayarla
                    coinsPerFormation,
                    arcHeight,
                    spawnDistance
                );

                // Platformda işgal edilen noktaları kontrol et
                if (!spawnPositions.Any(p => platform.IsPointOccupied(p)))
                {
                    SpawnFormation(spawnPositions, platform);

                    // İşaretleme
                    foreach (Vector3 pos in spawnPositions)
                    {
                        if (!platform.IsPointOccupied(pos))
                        {
                            // Spawn işlemi
                            platform.MarkPointOccupied(pos);
                        }
                    }
                }
            }
        }
    }

    private void SpawnFormation(List<Vector3> positions, Platform platform)
    {
        foreach (Vector3 pos in positions)
        {
            GameObject collectible = GetObjectFromPool();
            if (collectible != null)
            {
                collectible.transform.position = pos;
                collectible.SetActive(true); // Objeyi aktif hale getir
                activeObjects.Enqueue(collectible);
                if (!platform.IsPointOccupied(pos))
                {
                    // Spawn işlemi
                    platform.MarkPointOccupied(pos);
                }
            }
        }
    }

    // For existing code compatibility
    public void ClearCollectibles(GameObject platform)
    {
        ClearObjects(platform);
    }
    
    // Override the base class method
    public override void ClearObjects(GameObject platform)
    {
        if (platform == null) return;

        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
        {
            platformScript.ClearOccupiedPoints();
        }

        float platformMinZ = platform.transform.position.z - platform.transform.localScale.z / 2;
        float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z / 2;

        int itemsToProcess = activeObjects.Count;
        for (int i = 0; i < itemsToProcess; i++)
        {
            GameObject obj = activeObjects.Dequeue();
            if (obj.transform.position.z >= platformMinZ &&
                obj.transform.position.z <= platformMaxZ)
            {
                ReturnObjectToPool(obj);
            }
            else
            {
                activeObjects.Enqueue(obj);
            }
        }
    }

    // For existing code compatibility
    public void ClearAllCollectibles()
    {
        ClearObjects();
    }
    
    // Implement abstract method from BaseSpawner
    protected override GameObject GetObjectFromPool()
    {
        return PoolManager.Instance.Get(poolTag);
    }
    
    // Implement abstract method from BaseSpawner
    protected override void ReturnObjectToPool(GameObject obj)
    {
        PoolManager.Instance.Return(obj);
    }
}
