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

            Vector3 spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            availablePoints.Remove(spawnPoint);

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
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("ObstacleHalf");

        foreach (GameObject obstacle in obstacles)
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
