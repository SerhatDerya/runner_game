using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : BaseSpawner<CollectibleSpawner, CollectiblePool>
{
    [SerializeField] private int formationCount = 10;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private int coinsPerFormation = 5;

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

            List<Vector3> spawnPositions = CoinPattern.GetLinePattern(
                new Vector3(lanePositionX, CalculateSpawnHeight(platform), spawnZ),
                coinsPerFormation,
                spawnDistance
            );

            foreach (Vector3 pos in spawnPositions)
            {
                GameObject collectible = GetObjectFromPool();

                if (collectible != null) // Null kontrolü eklendi
                {
                    collectible.transform.position = pos;
                    activeObjects.Enqueue(collectible);
                }
                else
                {
                    Debug.LogWarning("Failed to get collectible from pool for position: " + pos);
                }
            }
        }


    }

    protected override void ReturnObjectToPool(GameObject obj)
    {
        objectPool.ReturnToPool(obj); // objectPool zaten CollectiblePool tipinde
    }

    protected override GameObject GetObjectFromPool()
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return null;
        }

        GameObject collectible = objectPool.GetCollectible();

        if (collectible == null)
        {
            Debug.LogWarning("No available collectibles in the pool!");
        }

        return collectible;
    }

public override void ClearObjects(GameObject platform)
{
    if (platform == null) return;

    float platformMinZ = platform.transform.position.z - platform.transform.localScale.z/2;
    float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z/2;
    
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
            activeObjects.Enqueue(obj); // Kalanları geri ekle
        }
    }
}
}
