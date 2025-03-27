using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Platform : MonoBehaviour
{
    public List<Vector3> spawnPoints; // Platform üzerindeki olası spawn noktaları
    private HashSet<Vector3> occupiedPoints = new HashSet<Vector3>(); // Kullanılan noktalar
    [SerializeField] private float spawnYOffset = 0.5f; // Spawn noktalarının yüksekliği

    private LaneManager laneManager;
    private int previousLaneIndex = -1; // Bir önceki formasyonun çıktığı şerit (-1 başlangıç değeri)
    private ObstacleManager obstacleManager = new ObstacleManager(); // Obstacle yönetimi için ObstacleManager
    private Dictionary<int, List<Vector3>> laneSpawnPoints = new Dictionary<int, List<Vector3>>(); // Şerit bazlı spawn noktaları

    void Awake()
    {
        laneManager = LaneManager.instance;
        if (laneManager == null)
        {
            Debug.LogError("LaneManager instance is missing!");
            return;
        }

        InitializeSpawnPoints();
    }

    private void InitializeSpawnPoints()
    {
        spawnPoints = new List<Vector3>();
        laneSpawnPoints.Clear();

        Collider platformCollider = GetComponent<Collider>();
        if (platformCollider == null)
        {
            Debug.LogError("Platform has no collider!");
            return;
        }

        float platformStartZ = transform.position.z - platformCollider.bounds.extents.z;
        float platformEndZ = transform.position.z + platformCollider.bounds.extents.z;
        int spawnRows = 10; // Kaç sıra olacağı

        float platformHeight = platformCollider.bounds.size.y; // Platformun yüksekliği

        for (int i = 0; i < spawnRows; i++)
        {
            float spawnZ = Mathf.Lerp(platformStartZ, platformEndZ, (float)i / spawnRows);

            for (int laneIndex = 0; laneIndex < laneManager.laneCount; laneIndex++)
            {
                float laneX = laneManager.GetLanePosition(laneIndex);
                Vector3 spawnPoint = new Vector3(laneX, transform.position.y + platformHeight * 0.5f + spawnYOffset, spawnZ);
                spawnPoints.Add(spawnPoint);

                // Şerit bazlı spawn noktalarını organize et
                if (!laneSpawnPoints.ContainsKey(laneIndex))
                {
                    laneSpawnPoints[laneIndex] = new List<Vector3>();
                }
                laneSpawnPoints[laneIndex].Add(spawnPoint);
            }
        }
    }

    public void ResetSpawnPoints()
    {
        // İşaretlenmiş noktaları temizle ve spawn noktalarını yeniden hesapla
        ClearOccupiedPoints();
        InitializeSpawnPoints();
    }

    public void MarkPointOccupied(Vector3 point)
    {
        occupiedPoints.Add(point);
    }

    public bool IsPointOccupied(Vector3 point)
    {
        return occupiedPoints.Contains(point);
    }

    public List<Vector3> GetAvailablePoints()
    {
        return spawnPoints.Where(p => !occupiedPoints.Contains(p)).ToList();
    }

    public List<Vector3> GetAvailablePointsInLane(int laneIndex)
    {
        if (!laneSpawnPoints.ContainsKey(laneIndex)) return new List<Vector3>();

        return laneSpawnPoints[laneIndex].Where(p => !occupiedPoints.Contains(p)).ToList();
    }

    public void ClearOccupiedPoints()
    {
        occupiedPoints.Clear();
    }

    public int GetNextLaneIndex()
    {
        // Eğer önceki şerit belirlenmemişse (ilk formasyon), rastgele bir şerit seç
        if (previousLaneIndex == -1)
        {
            previousLaneIndex = Random.Range(0, laneManager.laneCount);
            return previousLaneIndex;
        }

        // Sağdaki veya soldaki şeridi seç
        int nextLaneIndex;
        if (Random.value > 0.5f) // %50 ihtimalle sağdaki şerit
        {
            nextLaneIndex = Mathf.Min(previousLaneIndex + 1, laneManager.laneCount - 1);
        }
        else // %50 ihtimalle soldaki şerit
        {
            nextLaneIndex = Mathf.Max(previousLaneIndex - 1, 0);
        }

        previousLaneIndex = nextLaneIndex; // Yeni şeridi kaydet
        return nextLaneIndex;
    }

    public void AddObstaclePosition(int laneIndex, Vector3 position)
    {
        obstacleManager.AddObstacle(laneIndex, position.z);
    }

    public bool IsObstacleInLane(int laneIndex, float zPosition, float tolerance = 2f)
    {
        return obstacleManager.IsObstacleInLane(laneIndex, zPosition, tolerance);
    }

    public List<Vector3> GetSafeSpawnPoints(int laneIndex, float zPosition, float tolerance = 2f)
    {
        return GetAvailablePointsInLane(laneIndex)
            .Where(p => !IsObstacleInLane(laneIndex, p.z, tolerance))
            .ToList();
    }
}