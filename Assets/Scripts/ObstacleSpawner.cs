using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public void SpawnObstacles(GameObject platform, List<GameObject> obstaclesList)
    {
        int obstacleCount = Random.Range(5, 10);
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        float spawnAreaMinZ = platform.transform.position.z - 100f;
        float spawnAreaMaxZ = platform.transform.position.z + 100f;

        for (int i = 0; i < obstacleCount; i++)
        {
            float randomZ = Random.Range(spawnAreaMinZ, spawnAreaMaxZ);
            float randomY = platform.transform.position.y + platformHeight + 1f;
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            obstaclesList.Add(newObstacle); // Listeye ekle
        }
    }
}
