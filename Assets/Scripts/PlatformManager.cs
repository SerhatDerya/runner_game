using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;
    public CollectibleSpawner collectibleSpawner;
    public BuildingSpawner buildingSpawner;
    public PlatformSpawner platformSpawner;
    public Transform playerTransform;
    private int platformCount = 3;
    private int initialPlatformLength = 50;

    private Queue<GameObject> platformQueue = new();

    void Start()
    {
        GameObject initialPlatform = platformSpawner.SpawnInitialPlatform(initialPlatformLength);

        Vector3 firstPos = new Vector3(0, 0, initialPlatform.transform.position.z + initialPlatform.GetComponent<Collider>().bounds.size.z);
        GameObject firstPlatform = platformSpawner.SpawnPlatform(firstPos);
        platformQueue.Enqueue(firstPlatform);

        Vector3 secondPos = new Vector3(0, 0, firstPlatform.transform.position.z + firstPlatform.GetComponent<Collider>().bounds.size.z);
        GameObject secondPlatform = platformSpawner.SpawnPlatform(secondPos);
        platformQueue.Enqueue(secondPlatform);

        SpawnObjects(firstPlatform);
        SpawnObjects(secondPlatform);
    }

    void Update()
    {
        if (platformQueue.Count > 0)
        {
            GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];
            if (playerTransform.position.z >= lastPlatform.transform.position.z - 90)
                SpawnPlatform();

            DeactivateOldPlatform();
        }
    }

    public void SpawnPlatform()
{
    GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

    if (platformQueue.Count >= platformCount)
    {
        GameObject oldPlatform = platformQueue.Dequeue();
        ClearObjects(oldPlatform);
        Platform platform = oldPlatform.GetComponent<Platform>();
        platform?.ClearOccupiedPoints();

        // Platformu devre dışı bırak
        platformSpawner.SetPlatformActive(oldPlatform, false);

        float platformLength = oldPlatform.GetComponent<Collider>().bounds.size.z;
        Vector3 newPos = new Vector3(
            oldPlatform.transform.position.x,
            oldPlatform.transform.position.y,
            lastPlatform.transform.position.z + platformLength);

        platformSpawner.SetPlatformPosition(oldPlatform, newPos);

        // Platformu yeniden aktif hale getir
        platform?.ResetSpawnPoints();
        platformSpawner.SetPlatformActive(oldPlatform, true);

        platformQueue.Enqueue(oldPlatform);
        SpawnObjects(oldPlatform);
    }
    else
    {
        float platformLength = lastPlatform.GetComponent<Collider>().bounds.size.z;
        Vector3 newPos = new Vector3(
            lastPlatform.transform.position.x,
            lastPlatform.transform.position.y,
            lastPlatform.transform.position.z + platformLength);

        GameObject newPlatform = platformSpawner.SpawnPlatform(newPos);
        platformQueue.Enqueue(newPlatform);
        SpawnObjects(newPlatform);
    }
}

    void DeactivateOldPlatform()
{
    if (platformQueue.Count > 1)
    {
        GameObject oldPlatform = platformQueue.Peek();
        float platformLength = oldPlatform.GetComponent<Collider>().bounds.size.z;

        if (playerTransform.position.z >= oldPlatform.transform.position.z + platformLength)
        {
            platformSpawner.SetPlatformActive(oldPlatform, false);
            platformQueue.Dequeue(); // Platformu kuyruktan çıkar
        }
    }
}

    void SpawnObjects(GameObject platform)
    {
        obstacleSpawner.SpawnObstacles(platform);
        collectibleSpawner.SpawnCollectibles(platform);
        buildingSpawner.SpawnBuildings(platform);
    }

    void ClearObjects(GameObject platform)
    {
        obstacleSpawner.ClearObstacles(platform);
        collectibleSpawner.ClearCollectibles(platform);
        buildingSpawner.ClearBuildings(platform);
    }
}