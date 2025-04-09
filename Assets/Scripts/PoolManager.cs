using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolConfig
{
    public string tag;
    public List<GameObject> prefabs;
    public int size;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [SerializeField] private List<PoolConfig> poolConfigs;

    private Dictionary<string, List<Queue<GameObject>>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, List<Queue<GameObject>>>();

        foreach (var config in poolConfigs)
        {
            List<Queue<GameObject>> prefabPools = new List<Queue<GameObject>>();

            foreach (var prefab in config.prefabs)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < config.size; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);

                    PoolTag poolTag = obj.GetComponent<PoolTag>();
                    if (poolTag == null)
                    {
                        poolTag = obj.AddComponent<PoolTag>();
                    }

                    // Etiketi ekle
                    if (!poolTag.poolTags.Contains(config.tag))
                    {
                        poolTag.poolTags.Add(config.tag);
                    }

                    objectPool.Enqueue(obj);
                }

                prefabPools.Add(objectPool);
            }

            poolDictionary[config.tag] = prefabPools;
        }
    }

    public GameObject Get(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        List<Queue<GameObject>> prefabPools = poolDictionary[tag];

        // Her prefab havuzunu sırayla kontrol et
        foreach (var prefabPool in prefabPools)
        {
            if (prefabPool.Count > 0)
            {
                GameObject obj = prefabPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }

        // Eğer tüm havuzlar boşsa, yeni prefab instantiate et
        var config = poolConfigs.Find(c => c.tag == tag);
        if (config == null || config.prefabs.Count == 0)
        {
            Debug.LogError($"No config found for tag: {tag}");
            return null;
        }

        // Rastgele bir prefab seç ve instantiate et
        GameObject prefab = config.prefabs[Random.Range(0, config.prefabs.Count)];
        GameObject newObj = Instantiate(prefab);

        PoolTag poolTag = newObj.GetComponent<PoolTag>() ?? newObj.AddComponent<PoolTag>();

        // Etiketi ekle
        if (!poolTag.poolTags.Contains(tag))
        {
            poolTag.poolTags.Add(tag);
        }

        newObj.SetActive(true);
        return newObj;
    }

    public void Return(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"No pool exists with tag: {tag}. Object will be deactivated but not destroyed.");
            obj.SetActive(false); // Obje yok edilmez, sadece devre dışı bırakılır
            return;
        }

        obj.SetActive(false);

        // Objeyi doğru prefab havuzuna geri döndür
        foreach (var prefabPool in poolDictionary[tag])
        {
            // PoolTag bileşeni objede var mı kontrol et
            PoolTag poolTag = obj.GetComponent<PoolTag>();
            if (poolTag != null && poolTag.poolTags.Contains(tag))
            {
                prefabPool.Enqueue(obj);
                return; // Objeyi bu havuza başarıyla geri döndürdük
            }
        }

        Debug.LogWarning($"Returned object doesn't match any prefab in the pool for tag: {tag}. Object will be deactivated but not destroyed.");
    }

    public void Return(GameObject obj)
    {
        PoolTag poolTag = obj.GetComponent<PoolTag>();
        if (poolTag == null)
        {
            Debug.LogWarning("GameObject missing PoolTag component. Object will be deactivated but not destroyed.");
            obj.SetActive(false); // Obje yok edilmez, sadece devre dışı bırakılır
            return;
        }

        // İlk etiketi kullanarak geri döndür
        if (poolTag.poolTags.Count > 0)
        {
            Return(poolTag.poolTags[0], obj);
        }
        else
        {
            Debug.LogWarning("GameObject has no tags in PoolTag component. Object will be deactivated but not destroyed.");
            obj.SetActive(false); // Obje yok edilmez, sadece devre dışı bırakılır
        }
    }    
}
