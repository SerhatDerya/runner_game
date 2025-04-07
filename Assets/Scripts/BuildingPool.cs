using UnityEngine;
using System.Collections.Generic;

public class BuildingPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> buildingPrefabs;
    [SerializeField] private int poolSizePerPrefab = 5;

    private Dictionary<GameObject, Queue<GameObject>> prefabPools = new Dictionary<GameObject, Queue<GameObject>>();
    
    public void InitializePool()
    {
        foreach (var prefab in buildingPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            prefabPools[prefab] = pool;
        }
    }

    public GameObject GetGameObject()
    {
        // Rastgele bir prefab seç
        GameObject selectedPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];

        if (prefabPools.TryGetValue(selectedPrefab, out var pool))
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                Debug.LogWarning("Pool is empty for prefab: " + selectedPrefab.name + ", instantiating new.");
                GameObject obj = Instantiate(selectedPrefab);
                obj.SetActive(false);
                return obj;
            }
        }

        Debug.LogError("Selected prefab not found in pool.");
        return null;
    }

    public void ReturnGameObject(GameObject obj)
    {
        obj.SetActive(false);
        
        foreach (var prefab in buildingPrefabs)
        {
            if (obj.name.Contains(prefab.name))
            {
                prefabPools[prefab].Enqueue(obj);
                return;
            }
        }

        Debug.LogWarning("Returned object doesn't match any prefab in the pool.");
    }
}