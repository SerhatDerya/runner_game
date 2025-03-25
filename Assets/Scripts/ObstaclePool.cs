using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : GenericObjectPool<ObstacleController>
{
    [SerializeField] private GameObject obstacleFullPrefab;
    [SerializeField] private GameObject obstacleHalfPrefab;
    [SerializeField] private float fullObstacleProbability = 0.6f;

    public override void AddNewObjectToPool()
    {
        // Rastgele engel tipini seç
        GameObject selectedPrefab = Random.Range(0f, 1f) <= fullObstacleProbability ? 
            obstacleFullPrefab : obstacleHalfPrefab;
        
        // Prefabı instanciate et ve ObstacleController bileşenini al
        ObstacleController obstacle = Instantiate(selectedPrefab).GetComponent<ObstacleController>();
        
        if (obstacle == null)
        {
            Debug.LogError($"Prefab does not have ObstacleController component! {selectedPrefab.name}");
            return;
        }
        
        // Nesneyi devre dışı bırak ve havuza ekle
        obstacle.gameObject.SetActive(false);
        //obstacle.transform.SetParent(transform);
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