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

    // Start is called before the first frame update
    void Start()
    {
        // İlk platformu oluşturuyoruz
        GameObject firstPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity);
        platformQueue.Enqueue(firstPlatform);
        currentPlatform = firstPlatform;

        // İlk platform için collectable ve obstacle'ları spawn et
        platformObstacles[firstPlatform] = new List<GameObject>();
        platformCollectables[firstPlatform] = new List<GameObject>();

        collectableSpawner.SpawnCollectables(firstPlatform, platformCollectables[firstPlatform]);
        obstacleSpawner.SpawnObstacles(firstPlatform, platformObstacles[firstPlatform]);

        SpawnPlatform(); // İlk platformu spawn et
    }

    void SpawnPlatform()
    {
        if (platformQueue.Count == platformCount)
        {
            GameObject oldPlatform = platformQueue.Dequeue();

            // Silinen platformun üzerindeki engelleri ve collectable'ları Destroy et
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

            Destroy(oldPlatform, 1f);
        }

        GameObject lastPlatform = platformQueue.Peek();
        float platformX = lastPlatform.transform.position.x;
        float platformY = lastPlatform.transform.position.y;
        float platformZ = lastPlatform.transform.position.z;
        Vector3 newPlatformPos = new Vector3(platformX, platformY, platformZ + 200);
        GameObject newPlatform = Instantiate(lastPlatform, newPlatformPos, Quaternion.identity);
        platformQueue.Enqueue(newPlatform);

        // Yeni platform için collectable ve obstacle listeleri
        platformObstacles[newPlatform] = new List<GameObject>();
        platformCollectables[newPlatform] = new List<GameObject>();

        // Yeni platform için collectable ve obstacle'ları spawn edip listelere ekle
        collectableSpawner.SpawnCollectables(newPlatform, platformCollectables[newPlatform]);
        obstacleSpawner.SpawnObstacles(newPlatform, platformObstacles[newPlatform]);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];

        if (playerTransform.position.z >= lastPlatform.transform.position.z - 50)
        {
            SpawnPlatform();
        }
    }
}
