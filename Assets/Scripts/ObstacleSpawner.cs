using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : BaseSpawner<ObstacleController, ObstaclePool>
{
    private Dictionary<float, int> laneObstacleCounts = new Dictionary<float, int>(); // Her lane'deki engel sayısını takip eder

    public override void SpawnObjects(GameObject platformObj)
    {
        // Null kontrolü
        if (platformObj == null)
        {
            Debug.LogError("Platform object is null!");
            return;
        }

        Platform platform = platformObj.GetComponent<Platform>();
        if (platform == null)
        {
            Debug.LogError("Platform script is missing!");
            return;
        }

        List<Vector3> availablePoints = platform.GetAvailablePoints();

        // Eğer availablePoints boşsa, spawn işlemini durdur
        if (availablePoints == null || availablePoints.Count == 0)
        {
            Debug.LogWarning("No available points on the platform for obstacle spawning!");
            return;
        }

        // Her lane'deki engel sayısını sıfırla
        laneObstacleCounts.Clear();
        
        // Her Z pozisyonundaki engel sayısını tutacak dictionary
        Dictionary<float, int> zPosObstacleCounts = new Dictionary<float, int>();

        // Mevcut platformda aktif olan tüm objeleri kontrol et ve onları spawn et
        foreach (Vector3 spawnPoint in availablePoints)
        {
            if (Random.value <= spawnChance) // Rastgele spawn şansı
            {
                float laneX = spawnPoint.x;
                float laneZ = spawnPoint.z;
                
                // Aynı Z ekseninde (milestone'da) 4'ten fazla engel olup olmadığını kontrol et
                if (!zPosObstacleCounts.ContainsKey(laneZ))
                {
                    zPosObstacleCounts[laneZ] = 0;
                }
                else if (zPosObstacleCounts[laneZ] >= (laneManager.laneCount-1))
                {
                    // Bu Z pozisyonunda (şerit sayısı - 1) engel varsa, yeni engel ekleme
                    continue;
                }

                GameObject obstacle = GetObjectFromPool(); // Pool'dan engel al
                if (obstacle != null)
                {
                    // Engelin Collider'ını al ve spawn yüksekliğini hesapla
                    Collider obstacleCollider = obstacle.GetComponent<Collider>();
                    if (obstacleCollider != null)
                    {
                        float obstacleHeight = obstacleCollider.bounds.size.y;
                        float spawnHeight = platform.transform.position.y; // Platformun ortasında

                        // Engel pozisyonunu belirle
                        obstacle.transform.position = new Vector3(spawnPoint.x, spawnHeight, spawnPoint.z);
                        obstacle.SetActive(true);
                        activeObjects.Enqueue(obstacle); // Aktif engeller listesine ekle

                        // Platformdaki bu noktayı işaretle
                        platform.MarkPointOccupied(spawnPoint);

                        // Bu lane'deki engel sayısını artır
                        if (!laneObstacleCounts.ContainsKey(laneX))
                        {
                            laneObstacleCounts[laneX] = 0;
                        }
                        laneObstacleCounts[laneX]++;
                        
                        // Bu Z pozisyonundaki engel sayısını artır
                        zPosObstacleCounts[laneZ]++;
                    }
                }
            }
        }
    }

    public override void ClearObjects(GameObject platform)
    {
        if (platform == null) return;

        // Platformun sınırlarını al
        float platformMinZ = platform.transform.position.z - platform.transform.localScale.z / 2;
        float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z / 2;

        int itemsToProcess = activeObjects.Count;
        for (int i = 0; i < itemsToProcess; i++)
        {
            GameObject obj = activeObjects.Dequeue();
            if (obj != null && obj.transform.position.z >= platformMinZ &&
                obj.transform.position.z <= platformMaxZ)
            {
                // Platformdan dışarıda olanları pool'a geri al
                ReturnObjectToPool(obj);
            }
            else if (obj != null)
            {
                activeObjects.Enqueue(obj);
            }
        }
    }

    protected override GameObject GetObjectFromPool()
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return null;
        }

        GameObject obstacle = objectPool.GetGameObject(); // Havuzdan bir engel al

        if (obstacle == null)
        {
            Debug.LogWarning("No available obstacles in the pool!");
        }

        return obstacle;
    }

    protected override void ReturnObjectToPool(GameObject obj)
    {
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool reference is not set!");
            return;
        }
        obj.SetActive(false); // Objeyi pasif hale getir
        objectPool.ReturnGameObject(obj); // Havuzdaki objeyi geri ver
    }
}