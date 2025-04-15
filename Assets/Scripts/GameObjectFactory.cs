using UnityEngine;
using System.Collections.Generic;

public class GameObjectFactory : IFactory<GameObject>
{
    private List<GameObject> prefabs;
    private Transform parent;

    public GameObjectFactory(List<GameObject> prefabs, Transform parent = null)
    {
        this.prefabs = prefabs;
        this.parent = parent;
    }

    public virtual GameObject Create()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("No prefabs available in the factory!");
            return null;
        }

        // Rastgele bir prefab seç
        GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }
}