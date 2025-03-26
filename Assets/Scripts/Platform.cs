using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Platform : MonoBehaviour
{
    public List<Vector3> spawnPoints; // Platform üzerindeki olası spawn noktaları
    private HashSet<Vector3> occupiedPoints = new HashSet<Vector3>(); // Kullanılan noktalar
    [SerializeField] private float spawnYOffset = 0.5f; // Spawn noktalarının yüksekliği

    private LaneManager laneManager;

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

    public void ClearOccupiedPoints()
    {
        occupiedPoints.Clear();
    }
}