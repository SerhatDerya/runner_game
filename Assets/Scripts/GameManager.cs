using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;
    public static event Action<int> OnLaneChange; // şerit değişimi için event

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gamePauseCanvas;
    
    [SerializeField] private GameObject InGameButtons;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCoinsText;

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
            int currentScore = ScoreManager.instance.GetCurrentScore();
            scoreText.text = currentScore.ToString();
            int highScore = ScoreManager.instance.GetHighScore();

            if (CoinManager.instance != null && gameOverCoinsText != null)
            {
                gameOverCoinsText.text = CoinManager.instance.GetCurrentCoins().ToString();
            }

            highScoreText.text = "Highest Score : " + highScore.ToString();
            gameOverCanvas.SetActive(true);
            InGameButtons.SetActive(false);
            Debug.Log("Game Over!");
            Time.timeScale = 0f;
            // Oyunun bitmesiyle ilgili diğer işlemler (örneğin, UI güncelleme, ses çalma, vb.)
        }
    }

    public void GamePause()
    {
        Time.timeScale = 0f; // Oyunu duraklat
        InGameButtons.SetActive(false);
        gamePauseCanvas.SetActive(true);
        // Oyunun duraklatılmasıyla ilgili diğer işlemler (örneğin, UI güncelleme, ses çalma, vb.)
    }

    public void GameResume()
    {
        Time.timeScale = 1f; // Oyunu devam ettir
        InGameButtons.SetActive(true);
        gamePauseCanvas.SetActive(false);
        // Oyunun devam etmesiyle ilgili diğer işlemler (örneğin, UI güncelleme, ses çalma, vb.)
    }

    public void RestartGame()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        if (CoinManager.instance != null)
        {
            CoinManager.instance.ResetSessionCoins();
        }

        SceneManager.LoadScene("GameScene");
        gameOverCanvas.SetActive(false);
        gamePauseCanvas.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1f; // Oyunu devam ettir
        // Yeniden başlatma işlemleri (örneğin, sahneyi yeniden yükleme)
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameOverCanvas.SetActive(false);
        gamePauseCanvas.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1f; // Oyunu devam ettir
        // Ana menüye dönme işlemleri (örneğin, sahneyi yeniden yükleme)
    }
}
