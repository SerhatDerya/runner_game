using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResponsiveUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image coinImage;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject pauseButton;

    private void Start()
    {
        AdjustInGameUI();
    }

    private void AdjustInGameUI()
    {
        Rect safeArea = Screen.safeArea; // Cihazın güvenli alanını al
        RectTransform rectTransformScore = scoreText.GetComponent<RectTransform>();
        RectTransform rectTransformCoin = coinImage.GetComponent<RectTransform>();
        RectTransform rectTransformCoinText = coinText.GetComponent<RectTransform>();
        RectTransform rectTransformPauseButton = pauseButton.GetComponent<RectTransform>();
       
        if(Screen.height > safeArea.yMax){
            // Çentik yüksekliğini hesapla
        float safeAreaOffset = Screen.height - safeArea.yMax; // Çentik yüksekliği
        // Score Text'i ekranın üst ortasına sabitle
        rectTransformScore.anchoredPosition = new Vector2(0, -safeAreaOffset - 75); // 50 piksel aşağı kaydır
        // Coin Image'i ekranın sol üstüne sabitle
        rectTransformCoin.anchoredPosition = new Vector2(rectTransformCoin.anchoredPosition.x , -safeAreaOffset - 75); 
        // Coin Text'i Coin Image'in yanına sabitle
        rectTransformCoinText.anchoredPosition = new Vector2(rectTransformCoinText.anchoredPosition.x, -safeAreaOffset - 55);
        // Pause Button'u ekranın sağ üstüne sabitle
        rectTransformPauseButton.anchoredPosition = new Vector2(rectTransformPauseButton.anchoredPosition.x, -safeAreaOffset - 75);
        }
    }

}