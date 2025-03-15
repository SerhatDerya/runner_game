using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager instance;

    public int laneCount = 5;
    public float laneWidth = 3f;
    public Transform platform;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        UpdatePlatform();
    }

    private void UpdatePlatform()
    {
        if (platform != null)
        {
            platform.localScale = new Vector3(laneCount * laneWidth, platform.localScale.y, platform.localScale.z);
        }
    }

    public float GetLanePosition(int index)
    {
        if (laneCount <= 0) return 0f;
        float startX = -laneWidth * (laneCount - 1) / 2; // İlk şeridin x pozisyonu
        return startX + (index * laneWidth);
    }

    public float GetRandomLane()
    {
        return GetLanePosition(Random.Range(0, laneCount));
    }

    public int GetMiddleLaneIndex()
    {
        return laneCount / 2; // Şerit sayısına göre ortadaki index'i bul
    }

}
