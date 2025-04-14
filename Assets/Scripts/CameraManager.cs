using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // Inspector üzerinden atayacağınız virtual kameralar
    public CinemachineVirtualCamera frontCam;
    public CinemachineVirtualCamera playerFollowCam;
    public CinemachineVirtualCamera gameOverCam;
    private Quaternion initialGameOverCamRotation;
    private Vector3 initialFramingOffset;

    // Game over kamera ayarları:
    public int gameOverPriority = 20;
    public float initialFOV = 60f;
    public float targetFOV = 30f;
    public float zoomDuration = 1.5f; // (İsteğe bağlı) zoom efekti için kullanılabilir

    // Slow motion ayarı (isteğe bağlı)
    public float slowMotionScale = 0.3f;
    public float slowMotionDuration = 2.0f;

    private void OnEnable()
    {
        // Event'lere abone olunuyor.
        GameManager.OnGameOver += SwitchToGameOverCam;
        GameManager.OnPlayerStartedRunning += SwitchToFollowCam;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= SwitchToGameOverCam;
        GameManager.OnPlayerStartedRunning -= SwitchToFollowCam;
    }

    void Start()
    {
        // Mevcut ayarları saklıyoruz.
        initialGameOverCamRotation = gameOverCam.transform.rotation;
        CinemachineFramingTransposer framing = gameOverCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framing != null)
        {
            initialFramingOffset = framing.m_TrackedObjectOffset;
        }

        // Diğer başlangıç ayarları
        frontCam.Priority = 10;
        playerFollowCam.Priority = 5;
        gameOverCam.Priority = 0;
    }

    // Oyuncu koşmaya başladığında çağrılan metot
    public void SwitchToFollowCam()
    {
        Debug.Log("SwitchToFollowCam tetiklendi");
        playerFollowCam.Priority = 15;
        frontCam.Priority = 5;
    }

    // Game Over olduğunda çalışacak metot:
    public void SwitchToGameOverCam()
    {
        Debug.Log("SwitchToGameOverCam tetiklendi");

        // GameOver kamerasına yüksek öncelik ver
        gameOverCam.Priority = gameOverPriority;
        frontCam.Priority = 5;
        playerFollowCam.Priority = 5;

        // Kameranın oyuncuya daha yakın ve yukarıdan bakması için offset ayarlarını güncelliyoruz.
        // Burada CinemachineFramingTransposer bileşeni kullanıldığı varsayılıyor.
        CinemachineFramingTransposer framing = gameOverCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framing != null)
        {
            // Örneğin; oyuncuya yakın olmak için Z ekseninde -10'dan -4'e,
            // yukarıdan bakmak için Y ekseninde 5'ten 3'e ayarlıyoruz.
            framing.m_TrackedObjectOffset = new Vector3(0f, 3f, -4f);
            Debug.Log("GameOver kamera offset ayarlandı: " + framing.m_TrackedObjectOffset);
        }
        else
        {
            Debug.LogWarning("GameOver kamera için FramingTransposer bileşeni bulunamadı!");
        }

        // İsteğe bağlı: Slow motion efektini başlat (zamanı yavaşlat)
        StartCoroutine(ActivateSlowMotion());

        // Yakınlaşma (zoom) ve disorientasyon (baş dönmesi) efektini coroutine ile uygula.
        StartCoroutine(ApplyZoomAndShake());
    }

    // Slow motion efektini uygular: Game over anında zamanı yavaşlatır, sonra normale döndürür.
    private IEnumerator ActivateSlowMotion()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * slowMotionScale;
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
    }

    // Bu coroutine game over kamerasına geçer geçmez uygulanır.
    // İlk aşamada 1 saniye boyunca shake (noise) ve hafif rastgele rotasyon ile baş dönmesi efekti verir,
    // ardından 0.2 saniyelik fade out aşaması ile efekt tamamen kapatılır.
    private IEnumerator ApplyZoomAndShake()
    {
        float intenseDuration = 1.0f; // Shake yoğunluğu 1 saniye boyunca uygulanır.
        float fadeDuration = 0.2f;    // Fade out süresi, 0.2 saniyede etkiler sıfırlanır.

        // Cinemachine'in Noise bileşenini elde ediyoruz.
        CinemachineBasicMultiChannelPerlin noise = gameOverCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogWarning("CinemachineBasicMultiChannelPerlin bileşeni bulunamadı!");
            yield break;
        }
        // Varsayılan (başlangıç) değerler
        float initAmplitude = noise.m_AmplitudeGain;
        float initFrequency = noise.m_FrequencyGain;

        // Hedef değerler: Hafif bir baş dönmesi hissi için
        float targetAmplitude = 2f;   // Önerilen hafif amplitude değeri
        float targetFrequency = 4f;   // Önerilen hafif frekans değeri

        // 1. Aşama: Kameraya geçer geçmez baştan shake (noise) ve hafif rastgele rotasyon uygula.
        noise.m_AmplitudeGain = targetAmplitude;
        noise.m_FrequencyGain = targetFrequency;

        // Rotasyon disorientasyonu için orijinal rotasyonu sakla.
        Quaternion originalRotation = gameOverCam.transform.rotation;
        float intenseElapsed = 0f;
        while (intenseElapsed < intenseDuration)
        {
            intenseElapsed += Time.deltaTime;
            float randomX = Random.Range(-3f, 3f);
            float randomY = Random.Range(-3f, 3f);
            float randomZ = Random.Range(-3f, 3f);
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
            gameOverCam.transform.rotation = Quaternion.Slerp(originalRotation, originalRotation * randomRotation, 0.5f);
            yield return null;
        }

        // 2. Aşama: Fade out – 0.2 saniye içinde shake ve rotasyon etkisini kademeli olarak normale döndür.
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float t = fadeElapsed / fadeDuration;
            noise.m_AmplitudeGain = Mathf.Lerp(targetAmplitude, 0f, t);
            noise.m_FrequencyGain = Mathf.Lerp(targetFrequency, 0f, t);
            gameOverCam.transform.rotation = Quaternion.Slerp(gameOverCam.transform.rotation, originalRotation, t);
            yield return null;
        }
        // Son olarak, shake efektini tamamen sıfırla.
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
        gameOverCam.transform.rotation = originalRotation;
    }

    public void ResetCameraSettings()
    {
        Time.timeScale = 1f;
        // Öncelikleri yeniden ayarla
        frontCam.Priority = 10;
        playerFollowCam.Priority = 5;
        gameOverCam.Priority = 0; // Resetleyerek gameOver kamerasını pasife alıyoruz

        // GameOver kamera transformunu resetle
        gameOverCam.transform.rotation = initialGameOverCamRotation;

        // Noise ayarlarını sıfırla
        CinemachineBasicMultiChannelPerlin noise = gameOverCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }

        // Eğer offsette değişiklik yaptıysanız, bunu da resetleyebilirsiniz.
        CinemachineFramingTransposer framing = gameOverCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framing != null)
        {
            framing.m_TrackedObjectOffset = initialFramingOffset;
        }
    }
}