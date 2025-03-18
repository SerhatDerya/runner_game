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
        //Debug.Log(collectiblePool.objectPool.Count);
        // İlk platformun oluşturulması
        GameObject firstPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity);
        platformQueue.Enqueue(firstPlatform);

        collectiblePool.InitializePool();
        obstaclePool.InitializePool();
        
        collectibleSpawner.SpawnCollectibles(firstPlatform);
        obstacleSpawner.SpawnObstacles(firstPlatform);

        SpawnPlatform(); // İlk platformun spawn edilmesi
        //Debug.Log(collectiblePool.objectPool.Count);
    }

    void SpawnPlatform()
    {
        // En son platformu bul (kuyruğun sonundaki)
        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];
        
        if (platformQueue.Count == platformCount)
        {
            // En eski platformu (kuyruğun başındaki) al
            GameObject oldPlatform = platformQueue.Dequeue();

            // Platformu yeni pozisyona taşı (Son platformun 200 birim ilerisine)
            float newZ = lastPlatform.transform.position.z + 200;
            oldPlatform.transform.position = new Vector3(
                oldPlatform.transform.position.x, 
                oldPlatform.transform.position.y, 
                newZ
            );

            // Platformun üzerindeki engelleri ve collectible'ların pozisyonunu değiştir
            obstacleSpawner.ChangeObstaclePosition(oldPlatform);
            collectibleSpawner.ChangeCollectiblePosition(oldPlatform);

            
            // Taşınan platformu kuyruğun sonuna ekle
            platformQueue.Enqueue(oldPlatform);

            
            // Yeniden konumlandırılan platform için collectible ve obstacle'ları spawn et
            collectibleSpawner.SpawnCollectibles(oldPlatform);
            obstacleSpawner.SpawnObstacles(oldPlatform);
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
            collectibleSpawner.SpawnCollectibles(newPlatform);
            obstacleSpawner.SpawnObstacles(newPlatform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(collectiblePool.objectPool.Count);
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