using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CollectableSpawner : MonoBehaviour
{
    public GameObject collectablePrefab;

    public void SpawnCollectables(GameObject platform, List<GameObject> collectablesList)
    {
        int collectableCount = Random.Range(5, 10);
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        float spawnAreaMinZ = platform.transform.position.z - 100f;
        float spawnAreaMaxZ = platform.transform.position.z + 100f;

        for (int i = 0; i < collectableCount; i++)
        {
            float randomZ = Random.Range(spawnAreaMinZ, spawnAreaMaxZ);
            float randomY = platform.transform.position.y + platformHeight + 1f;
            float lanePositionX = LaneManager.instance.GetRandomLane();

            Vector3 spawnPosition = new Vector3(lanePositionX, randomY, randomZ);

            GameObject newCollectable = Instantiate(collectablePrefab, spawnPosition, Quaternion.identity);
            collectablesList.Add(newCollectable); // Listeye ekle
        }
    }
}
