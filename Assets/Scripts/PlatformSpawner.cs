using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;

    public GameObject SpawnPlatform(Vector3 position)
    {
        return Instantiate(platformPrefab, position, Quaternion.identity);
    }

    public GameObject SpawnInitialPlatform(int initialPlatformLength)
    {
        return Instantiate(platformPrefab, new Vector3(0, 0, -initialPlatformLength), Quaternion.identity);
    }

    public void SetPlatformActive(GameObject platform, bool isActive)
    {
        platform.SetActive(isActive);
    }

    public void SetPlatformPosition(GameObject platform, Vector3 newPosition)
    {
        platform.transform.position = newPosition;
    }
}
