using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollectibleSpawner : BaseSpawner<CollectibleSpawner, CollectiblePool>
{
    [SerializeField] private int formationCount = 10;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private int coinsPerFormation = 5;
    [SerializeField] private float arcHeight = 2f;

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

            if (!positions.Any(p => platform.IsPointOccupied(p)))
            {
                SpawnFormation(positions, platform);
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

                float obstacleHeight = obstacleCollider.bounds.size.y;
                Vector3 obstaclePos = obstacle.transform.position;

                List<Vector3> spawnPositions = CoinPattern.GetJumpArcPattern(
                    obstaclePos,
                    obstacleHeight,
                    coinsPerFormation,
                    arcHeight,
                    spawnDistance
                );

                if (!spawnPositions.Any(p => platform.IsPointOccupied(p)))
                {
                    SpawnFormation(spawnPositions, platform);
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
                collectible.SetActive(true);  // Objeyi aktif hale getir
                activeObjects.Enqueue(collectible);
                platform.MarkPointOccupied(pos);
            }
        }
    }

    protected override GameObject GetObjectFromPool()
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return null;
        }
        return objectPool.GetGameObject();
    }

    protected override void ReturnObjectToPool(GameObject obj)
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return;
        }
        obj.SetActive(false); // Objeyi pasif hale getir
        objectPool.ReturnGameObject(obj);  // Havuzdaki objeyi geri ver
    }

    public override void ClearObjects(GameObject platform)
    {
        if (platform == null) return;

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
}
