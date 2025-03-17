using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CollectibleSpawner : MonoBehaviour
{
    public CollectiblePool CollectiblePool;
    private Queue<GameObject> activeCollectibles = new Queue<GameObject>();

    public void SpawnCollectibles(GameObject platform)
    {
        int collectibleCount = CollectiblePool.poolSize/2;
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        float spawnAreaMinZ = platform.transform.position.z - 100f;
        float spawnAreaMaxZ = platform.transform.position.z + 100f;

        for (int i = 0; i < collectibleCount; i++)
        {
            float randomZ = Random.Range(spawnAreaMinZ, spawnAreaMaxZ);
            float randomY = platform.transform.position.y + platformHeight + 1f;
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            // Havuzdan collectible al ve pozisyonunu ayarla
            GameObject collectible = CollectiblePool.Instance.GetCollectible();
            collectible.transform.position = spawnPosition;
            activeCollectibles.Enqueue(collectible);
        }
    }

    public void ClearCollectibles()
    {
        int halfCount = activeCollectibles.Count / 2;
        
        // Return first half of obstacles to the pool
        for (int i = 0; i < halfCount; i++)
        {
            CollectiblePool.Instance.ReturnToPool(activeCollectibles.Dequeue());
        }
        
    }
    
    public void ChangeCollectiblePosition(GameObject platform)
    {
        // Önce tüm engellerin yarısını havuza geri gönder
        ClearCollectibles();
        
        // Sonra platform için yeni engeller oluştur
        SpawnCollectibles(platform);
    }
}
