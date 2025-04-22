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
    [SerializeField] private GameObject gameOverCanvasContent;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject InGameButtons;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCoinsText;
    [SerializeField] private float gameOverAnimationDuration = 0.25f;
    public static Action OnPlayerStartedRunning;

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

            // Initialize game over canvas with zero scale before showing it
            gameOverCanvasContent.transform.localScale = Vector3.zero;
            gameOverCanvas.SetActive(true);
            // Add half second delay before starting animation
            StartCoroutine(AnimateGameOverCanvas(0.7f));
            InGameButtons.SetActive(false);
            Debug.Log("Game Over!");
            playerController.SwitchToFallenCollider();
            playerController.OnGameOver();
            OnGameOver?.Invoke();
        }
    }

    private IEnumerator AnimateGameOverCanvas(float delay)
    {
        float elapsedTime = 0f;
        yield return new WaitForSeconds(delay);
        while (elapsedTime < gameOverAnimationDuration)
        {
            // Calculate the current scale based on animation progress
            float t = elapsedTime / gameOverAnimationDuration;
            float scaleValue = Mathf.SmoothStep(0, 1, t);

            // Apply the scale
            gameOverCanvasContent.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

            // Update time
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure final scale is exactly 1
        gameOverCanvasContent.transform.localScale = Vector3.one;
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

        playerController.ResetState();
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
