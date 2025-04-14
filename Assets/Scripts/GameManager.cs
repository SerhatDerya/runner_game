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
    public static event Action OnGamePause;  // Added event for game pause
    public static event Action OnGameResume; // Added event for game resume
    public static event Action OnGameOver;   // Existing event 

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gamePauseCanvas;
    [SerializeField] private PlayerMovementController playerController;
    [SerializeField] private GameObject InGameButtons;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCoinsText;
    public static Action OnPlayerStartedRunning;

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
            TriggerGameOver(); // Call StopMovement on the player controller
        }
    }

    private IEnumerator FreezeTimeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void GamePause()
    {
        OnGamePause?.Invoke();
        InGameButtons.SetActive(false);
        gamePauseCanvas.SetActive(true);
    }

    public void GameResume()
    {
        OnGameResume?.Invoke();
        InGameButtons.SetActive(true);
        gamePauseCanvas.SetActive(false);
    }

    public void RestartGame()
    {
        CameraManager camManager = FindObjectOfType<CameraManager>();
        if (camManager != null)
        {
            camManager.ResetCameraSettings();
        }
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
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameOverCanvas.SetActive(false);
        gamePauseCanvas.SetActive(false);
        isGameOver = false;
    }
}
