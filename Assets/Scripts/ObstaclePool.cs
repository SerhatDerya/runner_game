using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : GenericObjectPool<ObstacleController>
{
    [SerializeField] private List<GameObject> obstacleFullPrefabs;
    [SerializeField] private List<GameObject> obstacleHalfPrefabs;
    [SerializeField] private float fullObstacleProbability = 0.6f;

    public override void AddNewObjectToPool()
    {
        // Rastgele engel tipini seç
        bool isFullObstacle = Random.Range(0f, 1f) <= fullObstacleProbability;
        GameObject selectedPrefab;

        if (isFullObstacle)
        {
            selectedPrefab = obstacleFullPrefabs[Random.Range(0, obstacleFullPrefabs.Count)];
        }
        else
        {
            selectedPrefab = obstacleHalfPrefabs[Random.Range(0, obstacleHalfPrefabs.Count)];
        }

        // Prefabı instantiate et ve ObstacleController bileşenini al
        ObstacleController obstacle = Instantiate(selectedPrefab).GetComponent<ObstacleController>();

        if (obstacle == null)
        {
            Debug.LogError($"Prefab does not have ObstacleController component! {selectedPrefab.name}");
            return;
        }

        // Nesneyi devre dışı bırak ve havuza ekle
        obstacle.gameObject.SetActive(false);
        objectPool.Enqueue(obstacle);
    }

    public GameObject GetGameObject()
    {
        ObstacleController controller = GetObject();
        return controller?.gameObject;
    }

    public void ReturnGameObject(GameObject obj)
    {
        ObstacleController controller = obj.GetComponent<ObstacleController>();
        if (controller != null)
        {
            ReturnToPool(controller);
        }
        else
        {
            Debug.LogError($"GameObject does not have ObstacleController component!");
        }
    }
}