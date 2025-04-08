using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    AudioManager audioManager;
    private bool isGameOver = false;
    public static event Action<int> OnLaneChange; // şerit değişimi için event

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gamePauseCanvas;
    [SerializeField] private PlayerMovementController playerController;
    [SerializeField] private GameObject InGameButtons;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCoinsText;
    public static event Action OnGameOver;

    public static void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }

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

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            audioManager.PlaySFX(audioManager.pain);
            isGameOver = true;
            int currentScore = ScoreManager.instance.GetCurrentScore();
            scoreText.text = currentScore.ToString();
            int highScore = ScoreManager.instance.GetHighScore();

            if (CoinManager.instance != null && gameOverCoinsText != null)
            {
                gameOverCoinsText.text = CoinManager.instance.GetCurrentCoins().ToString();
            }

            highScoreText.text = "Highest Score: " + highScore.ToString();
            gameOverCanvas.SetActive(true);
            InGameButtons.SetActive(false);
            Debug.Log("Game Over!");
            playerController.SwitchToFallenCollider();
            TriggerGameOver();
            StartCoroutine(FreezeTimeAfterDelay(1.0f));
        }
    }

    private IEnumerator FreezeTimeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // bu Time.timeScale = 1 olduğu sürece çalışır
        Time.timeScale = 0f;
        Debug.Log("Game Over - Time frozen.");
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
