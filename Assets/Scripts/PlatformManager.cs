using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;
    public CollectibleSpawner collectibleSpawner;
    public CollectiblePool collectiblePool;
    public ObstaclePool obstaclePool;
    public PlatformSpawner platformSpawner;
    public Transform playerTransform;
    private int platformCount = 2;
    private int initialPlatformLength = 50;

    private Queue<GameObject> platformQueue = new();

    void Start()
    {

        // Başlangıç platformunu spawn et
        platformSpawner.SpawnInitialPlatform(initialPlatformLength);

        // İlk platformu spawn et
        GameObject firstPlatform = platformSpawner.SpawnPlatform(new Vector3(0, 0, initialPlatformLength));
        platformQueue.Enqueue(firstPlatform);

        // İkinci platformu spawn et
        GameObject secondPlatform = platformSpawner.SpawnPlatform(new Vector3(0, 0, 200+initialPlatformLength));
        platformQueue.Enqueue(secondPlatform);

        // Pool'ları başlat
        if (collectiblePool != null)
        {
            collectiblePool.InitializePool();
        }
        else
        {
            Debug.LogError("CollectiblePool reference is not set!");
        }

        if (obstaclePool != null)
        {
            obstaclePool.InitializePool();
        }
        else
        {
            Debug.LogError("ObstaclePool reference is not set!");
        }

        // İlk iki platforma obje spawn et
        SpawnObjects(firstPlatform);
        SpawnObjects(secondPlatform);
    }

    public void SpawnPlatform()
    {
        if (platformQueue.Count == 0)
        {
            Debug.LogError("Platform queue is empty!");
            return;
        }

        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

        if (platformQueue.Count == platformCount)
        {
            // Eğer platform sayısı yeterliyse, eski platformu taşımak için işlem yap
            GameObject oldPlatform = platformQueue.Dequeue();
            obstacleSpawner.ClearObjects(oldPlatform);
            collectibleSpawner.ClearObjects(oldPlatform);

            Platform platform = oldPlatform.GetComponent<Platform>();
            if (platform != null)
            {
                platform.ClearOccupiedPoints(); // İşaretlenmiş noktaları temizle
            }

            // Yeni pozisyonu ayarla
            float platformLength = oldPlatform.GetComponent<Collider>().bounds.size.z;
            float newZ = lastPlatform.transform.position.z + platformLength;
            platformSpawner.SetPlatformPosition(oldPlatform, new Vector3(
                oldPlatform.transform.position.x,
                oldPlatform.transform.position.y,
                newZ
            ));

            // Spawn noktalarını yeniden hesapla
            if (platform != null)
            {
                platform.ResetSpawnPoints();
            }

            platformQueue.Enqueue(oldPlatform);
            SpawnObjects(oldPlatform);
        }
        else
        {
            // Yeni platform oluşturuluyor
            float platformLength = lastPlatform.GetComponent<Collider>().bounds.size.z;
            GameObject newPlatform = platformSpawner.SpawnPlatform(new Vector3(
                lastPlatform.transform.position.x,
                lastPlatform.transform.position.y,
                lastPlatform.transform.position.z + platformLength
            ));
            platformQueue.Enqueue(newPlatform);
            SpawnObjects(newPlatform);
        }
    }

    void Update()
    {
        if (platformQueue.Count > 0)
        {
            GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

            // Oyuncu, son platforma yaklaşınca yeni platform spawn et
            if (playerTransform.position.z >= lastPlatform.transform.position.z - 50)
            {
                SpawnPlatform();
            }

            DeactivateOldPlatform();
        }
    }

    void DeactivateOldPlatform()
{
    if (platformQueue.Count > 1)
    {
        GameObject oldPlatform = platformQueue.Peek(); // İlk platformu al
        float platformLength = oldPlatform.GetComponent<Collider>().bounds.size.z;

        // Oyuncu, platformun sonundan platform uzunluğu kadar uzaklaştığında platformu devre dışı bırak
        if (playerTransform.position.z >= oldPlatform.transform.position.z + platformLength)
        {
            platformSpawner.SetPlatformActive(oldPlatform, false);
        }
    }
}

    void SpawnObjects(GameObject platform)
    {
        obstacleSpawner.SpawnObjects(platform);
        collectibleSpawner.SpawnObjects(platform);
    }
}