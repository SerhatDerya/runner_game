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

    private Queue<GameObject> platformQueue = new();

    void Start()
    {
        // İlk platformu spawn et
        GameObject firstPlatform = platformSpawner.SpawnPlatform(Vector3.zero);
        platformQueue.Enqueue(firstPlatform);

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

        // İlk platforma obje spawn et
        SpawnObjects(firstPlatform);

        // İlk platformu spawn et
        SpawnPlatform(); 
    }

    public void SpawnPlatform()
    {
        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

        if (platformQueue.Count == platformCount)
        {
            // Eğer platform sayısı yeterliyse, eski platformu taşımak için işlem yap
            GameObject oldPlatform = platformQueue.Dequeue();
            obstacleSpawner.ClearObjects(oldPlatform);
            collectibleSpawner.ClearObjects(oldPlatform);

            // Yeni pozisyonu ayarla
            float newZ = lastPlatform.transform.position.z + 200;
            platformSpawner.SetPlatformPosition(oldPlatform, new Vector3(
                oldPlatform.transform.position.x,
                oldPlatform.transform.position.y,
                newZ
            ));

            // Eski platformu tekrar kuyruğa ekle (aktif bırak)
            platformQueue.Enqueue(oldPlatform);
            platformSpawner.SetPlatformActive(oldPlatform, true);  // Eski platform aktif kalmalı

            // Eski platformda nesneleri spawn et
            SpawnObjects(oldPlatform);
        }
        else
        {
            // Yeni platform oluşturuluyor
GameObject newPlatform = platformSpawner.SpawnPlatform(new Vector3(lastPlatform.transform.position.x, lastPlatform.transform.position.y, lastPlatform.transform.position.z + 200));
platformQueue.Enqueue(newPlatform);

// Yeni platform aktif yapılıyor
platformSpawner.SetPlatformActive(newPlatform, true);  // Bu satır çok önemli

// Yeni platformda nesneleri spawn et
SpawnObjects(newPlatform);  // Burada da SpawnObjects doğru şekilde çağrılmalı
 // Yeni platformu aktif hale getir
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
        GameObject oldPlatform = platformQueue.Peek();  // İlk platformu al
        if (playerTransform.position.z >= oldPlatform.transform.position.z + 120)
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
