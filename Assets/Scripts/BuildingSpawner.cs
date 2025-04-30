using UnityEngine;
using System.Collections.Generic;

public class BuildingSpawner : BaseSpawner<MonoBehaviour, PoolManager>
{
    [SerializeField] private string buildingTag = "Building"; // Pool'dan alınacak prefab etiketi
    [SerializeField] private float buildingOffset = 3f; // Binaların platform kenarına uzaklığı
    [SerializeField] private int buildingsPerSide = 3; // Her kenarda kaç bina spawn edileceği
    [SerializeField] private float distanceFromEdge = 10f; // Platformun sağ ve sol kenarından uzaklık
    [SerializeField] private float minDistanceBetweenBuildings = 7f; // Binalar arasındaki minimum mesafe
    [SerializeField] private float maxXRange = 60f; // Binaların X eksenindeki geniş yayılma aralığı (Daha geniş bir range)
    
    // Use BaseSpawner's activeObjects but keep a reference for easier reading
    private Queue<GameObject> ActiveBuildings => activeObjects;
    private List<Vector3> spawnedPositions = new List<Vector3>(); // Çakışmayı önlemek için kullanılan pozisyon listesi
    
    // Existing method with the same name for compatibility
    public void SpawnBuildings(GameObject platformObj)
    {
        SpawnObjects(platformObj);
    }

    // Implementation of the abstract method from BaseSpawner
    public override void SpawnObjects(GameObject platformObj)
    {
        Platform platform = platformObj.GetComponent<Platform>();
        if (platform == null)
        {
            Debug.LogError("Platform script is missing!");
            return;
        }

        Collider platformCollider = platform.GetComponent<Collider>();
        if (platformCollider == null)
        {
            Debug.LogError("Platform has no collider!");
            return;
        }

        Vector3 center = platform.transform.position;
        float halfZ = platformCollider.bounds.extents.z; // Platformun Z eksenindeki yarısı
        float topY = platformCollider.bounds.max.y;

        // Platformun genişliği hesaplanır
        float platformWidth = platformCollider.bounds.extents.x;

        // Platformun altına binaları yerleştir
        PlaceBuildingsBelowPlatform(center, platformWidth, halfZ);
    }

    private void PlaceBuildingsBelowPlatform(Vector3 platformCenter, float platformWidth, float platformHalfZ)
    {
        spawnedPositions.Clear(); // Önceki pozisyonları temizle

        for (int i = 0; i < buildingsPerSide; i++)
        {
            // Sağ kenar için bina spawn et
            Vector3 rightPosition = GenerateNonOverlappingPosition(
                platformCenter.x + platformWidth + distanceFromEdge,
                platformCenter.z,
                platformHalfZ,
                platformCenter.y,
                true // Sağ taraf
            );
            SpawnBuildingAt(rightPosition);

            // Sol kenar için bina spawn et
            Vector3 leftPosition = GenerateNonOverlappingPosition(
                platformCenter.x - platformWidth - distanceFromEdge,
                platformCenter.z,
                platformHalfZ,
                platformCenter.y,
                false // Sol taraf
            );
            SpawnBuildingAt(leftPosition);
        }
    }

    private Vector3 GenerateNonOverlappingPosition(float platformEdgeX, float platformZ, float platformHalfZ, float platformY, bool isRightSide)
    {
        Vector3 position;
        int maxAttempts = 10; // Çakışmayı önlemek için maksimum deneme sayısı
        int attempts = 0;

        do
        {
            // Rastgele bir Z pozisyonu belirle
            float randomZOffset = Random.Range(-platformHalfZ, platformHalfZ);

            // Rastgele bir Y offset belirle (platformun altına yerleştirmek için)
            float randomYOffset = Random.Range(-30f, -20f);

            // Bina pozisyonunu kenarına göre ayarla
            float xPosition = isRightSide
                ? platformEdgeX + Random.Range(0f, buildingOffset) // Sağ kenar
                : platformEdgeX - Random.Range(0f, buildingOffset); // Sol kenar

            // Binaları daha dağınık ve uzak yapabilmek için
            // X pozisyonu için daha geniş bir range ekleyelim
            float randomXOffset = Random.Range(-maxXRange / 2, maxXRange / 2); // Daha geniş bir X ekseni range'i

            position = new Vector3(xPosition + randomXOffset, platformY + randomYOffset, platformZ + randomZOffset);

            attempts++;
        }
        while (IsOverlapping(position) && attempts < maxAttempts);

        // Pozisyonu kaydet
        spawnedPositions.Add(position);

        return position;
    }

    private bool IsOverlapping(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minDistanceBetweenBuildings)
            {
                return true; // Çakışma var
            }
        }
        return false; // Çakışma yok
    }

    private void SpawnBuildingAt(Vector3 position)
    {
        GameObject building = GetObjectFromPool();
        if (building != null)
        {
            building.transform.position = position;
            building.SetActive(true); // Bina aktif et

            // SpawnScaleEffect scriptini tetikle
            SpawnScaleEffect scaleEffect = building.GetComponent<SpawnScaleEffect>();
            if (scaleEffect != null)
            {
                scaleEffect.enabled = false; // Script'i sıfırla
                scaleEffect.enabled = true; // Script'i yeniden etkinleştir
            }
        }
    }

    // For existing code compatibility
    public void ClearBuildings(GameObject platform)
    {
        ClearObjects(platform);
    }
    
    // Override the base class method
    public override void ClearObjects(GameObject platform)
    {
        float platformMinZ = platform.transform.position.z - platform.transform.localScale.z / 2;
        float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z / 2;

        int count = activeObjects.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject building = activeObjects.Dequeue();
            if (building.transform.position.z >= platformMinZ && building.transform.position.z <= platformMaxZ)
            {
                building.SetActive(false);
                ReturnObjectToPool(building);
            }
            else
            {
                activeObjects.Enqueue(building);
            }
        }
    }

    // Implement abstract method from BaseSpawner
    protected override GameObject GetObjectFromPool()
    {
        return PoolManager.Instance.Get(buildingTag);
    }

    // Implement abstract method from BaseSpawner
    protected override void ReturnObjectToPool(GameObject obj)
    {
        PoolManager.Instance.Return(buildingTag, obj);
    }
}
