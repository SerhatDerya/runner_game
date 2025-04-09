using UnityEngine;

public class GameObjectFactory : IFactory<GameObject>
{
    private GameObject prefab;
    private Transform parent;

    public GameObjectFactory(GameObject prefab, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public GameObject Create()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }
}
