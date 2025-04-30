using UnityEngine;

public class PlatformSpawner : BaseSpawner<MonoBehaviour, MonoBehaviour>
{
    public GameObject platformPrefab;

    // For BaseSpawner implementation
    public override void SpawnObjects(GameObject platform)
    {
        // This isn't directly used in PlatformSpawner's workflow but is implemented
        // to satisfy the abstract method requirement
        Debug.Log("SpawnObjects called in PlatformSpawner");
    }

    // Original methods kept for backward compatibility
    public GameObject SpawnPlatform(Vector3 position)
    {
        return Instantiate(platformPrefab, position, Quaternion.identity);
    }

    public GameObject SpawnInitialPlatform(float length)
    {
        return Instantiate(platformPrefab, new Vector3(0, 0, -length), Quaternion.identity);
    }

    public void SetPlatformActive(GameObject platform, bool isActive)
    {
        platform.SetActive(isActive);
    }

    public void SetPlatformPosition(GameObject platform, Vector3 newPosition)
    {
        platform.transform.position = newPosition;
    }

    // BaseSpawner abstract methods implementation
    protected override GameObject GetObjectFromPool()
    {
        // PlatformSpawner doesn't use pooling, but we need to implement this method
        // Uses Instantiate instead of pool
        return Instantiate(platformPrefab);
    }

    protected override void ReturnObjectToPool(GameObject obj)
    {
        // PlatformSpawner doesn't use pooling, so we destroy the object when "returning" it
        Destroy(obj);
    }
}