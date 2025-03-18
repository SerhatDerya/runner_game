using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;
    public static event Action<int> OnLaneChange; // şerit değişimi için event

    [SerializeField] private GameObject gameOverCanvas;

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

    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            gameOverCanvas.SetActive(true);
            Debug.Log("Game Over!");
            Time.timeScale = 0f;
            // Oyunun bitmesiyle ilgili diğer işlemler (örneğin, UI güncelleme, ses çalma, vb.)
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
        gameOverCanvas.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1f; // Oyunu devam ettir
        // Yeniden başlatma işlemleri (örneğin, sahneyi yeniden yükleme)
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameOverCanvas.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1f; // Oyunu devam ettir
        // Ana menüye dönme işlemleri (örneğin, sahneyi yeniden yükleme)
    }
}
