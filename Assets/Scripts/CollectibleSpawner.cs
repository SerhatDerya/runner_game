using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : BaseSpawner<CollectibleSpawner, CollectiblePool>
{
    [SerializeField] private int formationCount = 10;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private int coinsPerFormation = 5;
    [SerializeField] private float arcHeight = 2f;

    public override void SpawnObjects(GameObject platform)
    {
        Collider platformCollider = platform.GetComponent<Collider>();
        if (platformCollider == null)
        {
            Debug.LogError("Platform has no collider!");
            return;
        }

        float platformLength = platformCollider.bounds.size.z;
        float platformHeight = platformCollider.bounds.size.y;
        float platformStartZ = platform.transform.position.z - (platformLength / 2);
        float platformEndZ = platform.transform.position.z + (platformLength / 2);

        for (int i = 0; i < formationCount; i++)
        {
            float spawnZ = Random.Range(platformStartZ + 10f, platformEndZ - 10f);
            float lanePositionX = laneManager.GetRandomLane();

            // Formasyon tipi seç
            if (Random.value > 0.5f)
            {
                SpawnFormation(CoinPattern.GetLinePattern(
                    new Vector3(lanePositionX, CalculateSpawnHeight(platform), spawnZ),
                    coinsPerFormation,
                    spawnDistance
                ));
            }
        }

        // ObstacleHalf nesneleri için yay deseni oluştur
        SpawnOverObstacles();
    }

    private void SpawnOverObstacles()
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

            SpawnFormation(spawnPositions);
        }
    }

    private void SpawnFormation(List<Vector3> positions)
    {
        foreach (Vector3 pos in positions)
        {
            GameObject collectible = GetObjectFromPool();

            if (collectible != null)
            {
                collectible.transform.position = pos;
                activeObjects.Enqueue(collectible);
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

        GameObject collectible = objectPool.GetGameObject(); // Artık doğru metodu çağırıyoruz

        if (collectible == null)
        {
            Debug.LogWarning("No available collectibles in the pool!");
        }

        return collectible;
    }


    protected override void ReturnObjectToPool(GameObject obj)
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return;
        }

        objectPool.ReturnGameObject(obj); // Doğrudan `ReturnGameObject()` metodunu kullanıyoruz
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
