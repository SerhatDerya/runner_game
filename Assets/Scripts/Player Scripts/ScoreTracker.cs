using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public float distanceForScore = 1f;
    
    private Vector3 lastScoredPosition;
    private int currentScore;
    public static event System.Action<int> OnScoreChanged;

    public void Init(Vector3 startPosition)
    {
        lastScoredPosition = startPosition;
        currentScore = 0;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
    }

    public void TrackDistance(Vector3 playerPosition)
    {
        if (Vector3.Distance(playerPosition, lastScoredPosition) >= distanceForScore)
        {
            int deltaScore = Mathf.FloorToInt(Vector3.Distance(playerPosition, lastScoredPosition));
            currentScore += deltaScore;
            lastScoredPosition = playerPosition;

            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(deltaScore);
            }
            
            OnScoreChanged?.Invoke(currentScore);
        }
    }

    public int GetScore() => currentScore;
}