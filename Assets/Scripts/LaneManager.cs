using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager instance; // Singleton (Tüm scriptlerden erişim için)

    public float[] lanes = { -3f, 0f, 3f }; // Şeritlerin X konumları

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public float GetRandomLane() 
    {
        return lanes[Random.Range(0, lanes.Length)]; // Rastgele bir şerit döndür
    }
}
