using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    public GameObject collectablePrefab;
    public Transform platform;
    public int minCollectables = 5;
    public int maxCollectables = 10;

    public Vector3 spawnAreaMin;   
    public Vector3 spawnAreaMax;

    void Start()
    {
        SpawnCollectables();
    }

    void SpawnCollectables()
    {
        int collectableCount = Random.Range(minCollectables, maxCollectables);

        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        for (int i = 0; i < collectableCount; i++)
        {
            // Platform üzerinde rastgele bir pozisyon
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

            float randomY = platform.position.y + platformHeight + 1f; // Platformdan yükseklik

            // Şerit pozisyonunu almak için LaneManager kullanıyoruz
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            // spawn et
            Instantiate(collectablePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
