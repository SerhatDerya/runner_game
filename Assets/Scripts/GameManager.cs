using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLaneChange; // şerit değişimi için event

    void Start()
    {
        int middleLane = LaneManager.instance.GetMiddleLaneIndex();
        OnLaneChange?.Invoke(middleLane); // oyuncuya başlangıç şeridini gönder
    }
}
