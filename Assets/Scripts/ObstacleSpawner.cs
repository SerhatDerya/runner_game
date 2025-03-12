using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform platform;
    public int minObstacles = 5;
    public int maxObstacles = 10;

    public Vector3 spawnAreaMin;   
    public Vector3 spawnAreaMax;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        int obstacleCount = Random.Range(minObstacles, maxObstacles);

        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        for (int i = 0; i < obstacleCount; i++)
        {
            // Platform üzerinde rastgele bir pozisyon
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);

            float randomY = platform.position.y + platformHeight + 1f; // Platformdan yükseklik

            // Şerit pozisyonunu almak için LaneManager kullanıyoruz
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            // spawn et
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
