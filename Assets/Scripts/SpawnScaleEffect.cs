using UnityEngine;

public class SpawnScaleEffect : MonoBehaviour
{
    public float duration = 0.5f;
    private Vector3 originalScale;
    private bool hasBeenInstantiated = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (!hasBeenInstantiated)
        {
            // İlk kez sahneye geldiğinde (Instantiate anı)
            hasBeenInstantiated = true;
            return; // animasyon yok
        }

        // Object pool'dan tekrar aktif olduğunda animasyon yap
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, originalScale, duration).setEaseOutBack();
    }
}
