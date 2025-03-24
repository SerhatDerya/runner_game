using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;
    public CollectibleSpawner collectibleSpawner;
    public CollectiblePool collectiblePool;
    public ObstaclePool obstaclePool;
    public GameObject platformPrefab;
    public Transform playerTransform;
    private int platformCount = 2;

    private Queue<GameObject> platformQueue = new Queue<GameObject>();


    void Start()
    {
        // İlk platformun oluşturulması
        GameObject firstPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity);
        platformQueue.Enqueue(firstPlatform);

        collectiblePool.InitializePool();
        obstaclePool.InitializePool();
        
        obstacleSpawner.SpawnObjects(firstPlatform);
        collectibleSpawner.SpawnObjects(firstPlatform);
        

        SpawnPlatform(); // İlk platformun spawn edilmesi
    }

    void SpawnPlatform()
    {
        // En son platformu bul (kuyruğun sonundaki)
        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];
        
        if (platformQueue.Count == platformCount)
        {
            // En eski platformu (kuyruğun başındaki) al
            GameObject oldPlatform = platformQueue.Dequeue();
            obstacleSpawner.ClearObjects(oldPlatform);

            // Platformu yeni pozisyona taşı (Son platformun 200 birim ilerisine)
            float newZ = lastPlatform.transform.position.z + 200;
            oldPlatform.transform.position = new Vector3(
                oldPlatform.transform.position.x, 
                oldPlatform.transform.position.y, 
                newZ
            );
            GameObject newPlatform = oldPlatform;

            // Taşınan platformu kuyruğun sonuna ekle
            platformQueue.Enqueue(newPlatform);

            // Platformun üzerindeki engelleri ve collectible'ların pozisyonunu değiştir
            obstacleSpawner.SpawnObjects(newPlatform);
            collectibleSpawner.ChangeObjectPositions(newPlatform);

        }
        else
        {
            // Henüz yeterli platform yoksa, yeni bir platform oluştur
            float platformX = lastPlatform.transform.position.x;
            float platformY = lastPlatform.transform.position.y;
            float newZ = lastPlatform.transform.position.z + 200;
            
            Vector3 newPlatformPos = new Vector3(platformX, platformY, newZ);
            GameObject newPlatform = Instantiate(platformPrefab, newPlatformPos, Quaternion.identity);
            platformQueue.Enqueue(newPlatform);


            // Yeni platform için collectible ve obstacle'ları spawn et
            obstacleSpawner.SpawnObjects(newPlatform);
            collectibleSpawner.SpawnObjects(newPlatform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (platformQueue.Count > 0)
        {
            GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

            if (playerTransform.position.z >= lastPlatform.transform.position.z - 50)
            {
                SpawnPlatform();
            }
        }
    }
}