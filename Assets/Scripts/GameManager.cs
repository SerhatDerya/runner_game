using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;
    public static event Action<int> OnLaneChange; // şerit değişimi için event

    void Start()
    {
        int middleLane = LaneManager.instance.GetMiddleLaneIndex();
        OnLaneChange?.Invoke(middleLane); // oyuncuya başlangıç şeridini gönder
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over!");
            Time.timeScale = 0f;
            // Oyunun bitmesiyle ilgili diğer işlemler (örneğin, UI güncelleme, ses çalma, vb.)
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Oyunu devam ettir
        // Yeniden başlatma işlemleri (örneğin, sahneyi yeniden yükleme)
    }
}
