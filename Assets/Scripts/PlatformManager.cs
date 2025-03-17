using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;
    public CollectableSpawner collectableSpawner;
    public GameObject platformPrefab;
    public Transform playerTransform;
    private GameObject currentPlatform;
    private int platformCount = 2;

    private Queue<GameObject> platformQueue = new Queue<GameObject>();

    private Dictionary<GameObject, List<GameObject>> platformObstacles = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> platformCollectables = new Dictionary<GameObject, List<GameObject>>();

    void Start()
    {
        // İlk platformun oluşturulması
        GameObject firstPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity);
        platformQueue.Enqueue(firstPlatform);
        currentPlatform = firstPlatform;

        // İlk platform için collectable ve obstacle'ların spawn edilmesi
        platformObstacles[firstPlatform] = new List<GameObject>();
        platformCollectables[firstPlatform] = new List<GameObject>();

        collectableSpawner.SpawnCollectables(firstPlatform, platformCollectables[firstPlatform]);
        obstacleSpawner.SpawnObstacles(firstPlatform, platformObstacles[firstPlatform]);

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

            // Platformun üzerindeki engelleri ve collectable'ları Destroy et
            if (platformObstacles.ContainsKey(oldPlatform))
            {
                foreach (GameObject obj in platformObstacles[oldPlatform])
                {
                    Destroy(obj);
                }
                platformObstacles.Remove(oldPlatform);
            }

            if (platformCollectables.ContainsKey(oldPlatform))
            {
                foreach (GameObject obj in platformCollectables[oldPlatform])
                {
                    Destroy(obj);
                }
                platformCollectables.Remove(oldPlatform);
            }
            
            // Platformu yeni pozisyona taşı (Son platformun 200 birim ilerisine)
            float newZ = lastPlatform.transform.position.z + 200;
            oldPlatform.transform.position = new Vector3(
                oldPlatform.transform.position.x, 
                oldPlatform.transform.position.y, 
                newZ
            );
            
            // Taşınan platformu kuyruğun sonuna ekle
            platformQueue.Enqueue(oldPlatform);
            
            // Yeni konumdaki platform için collectable ve obstacle listeleri
            platformObstacles[oldPlatform] = new List<GameObject>();
            platformCollectables[oldPlatform] = new List<GameObject>();

            // Yeniden konumlandırılan platform için collectable ve obstacle'ları spawn et
            collectableSpawner.SpawnCollectables(oldPlatform, platformCollectables[oldPlatform]);
            obstacleSpawner.SpawnObstacles(oldPlatform, platformObstacles[oldPlatform]);
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

            // Yeni platform için collectable ve obstacle listeleri
            platformObstacles[newPlatform] = new List<GameObject>();
            platformCollectables[newPlatform] = new List<GameObject>();

            // Yeni platform için collectable ve obstacle'ları spawn et
            collectableSpawner.SpawnCollectables(newPlatform, platformCollectables[newPlatform]);
            obstacleSpawner.SpawnObstacles(newPlatform, platformObstacles[newPlatform]);
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