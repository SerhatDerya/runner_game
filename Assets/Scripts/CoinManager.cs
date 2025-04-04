using UnityEngine;
using TMPro;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    
    [SerializeField] private TextMeshProUGUI coinText;
    
    private int currentCoins = 0;
    private int totalCoins = 0;
    
    // Event that other objects can subscribe to
    public static event Action<int> OnCoinsChanged;
    
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
        
        LoadTotalCoins();
    }
    
    private void Start()
    {
        UpdateCoinsUI();
    }
    
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        totalCoins += amount;
        
        OnCoinsChanged?.Invoke(currentCoins);
        UpdateCoinsUI();
        
        // Save total coins to PlayerPrefs
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
    }
    
    public int GetCurrentCoins()
    {
        return currentCoins;
    }
    
    public int GetTotalCoins()
    {
        return totalCoins;
    }
    
    public void ResetSessionCoins()
    {
        currentCoins = 0;
        UpdateCoinsUI();
    }
    
    private void UpdateCoinsUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
    
    private void LoadTotalCoins()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
    }
}