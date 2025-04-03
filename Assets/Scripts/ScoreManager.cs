using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    private int currentScore = 0;
    private int highScore = 0;
    private float scoreMultiplier = 1f;
    
    // Event that other objects can subscribe to
    public static event Action<int> OnScoreChanged;
    
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
        
        LoadHighScore();
    }
    
    private void Start()
    {
        UpdateScoreUI();
    }
    
    public void AddScore(int points)
    {
        currentScore += Mathf.RoundToInt(points * scoreMultiplier);
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
        
        if (currentScore > highScore)
        {
            SetHighScore(currentScore);
        }
    }
    
    public void AddScoreForDistance(float distance)
    {
        // Add 1 point per meter traveled
        int points = Mathf.FloorToInt(distance);
        if (points > 0)
        {
            AddScore(points);
        }
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    public void SetScoreMultiplier(float multiplier)
    {
        scoreMultiplier = multiplier;
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
        
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }
    }
    
    private void SetHighScore(int score)
    {
        highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
        UpdateScoreUI();
    }
    
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public int GetHighScore()
    {
        return highScore;
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}