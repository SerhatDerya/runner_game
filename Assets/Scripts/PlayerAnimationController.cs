using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementController playerMovement;
    private bool isPaused = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovementController>(); // Parent'taki hareket scriptini al
    }

    private void Update()
    {
        if (playerMovement == null) return;

        // Skip animation updates if game is paused
        if (isPaused) return;

        // Koşma Animasyonu
        animator.SetFloat("speed", playerMovement.forwardSpeed);

        // Zıplama Animasyonu
        animator.SetBool("isJumping", !playerMovement.isGrounded);
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGamePause += HandleGamePause;
        GameManager.OnGameResume += HandleGameResume;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGamePause -= HandleGamePause;
        GameManager.OnGameResume -= HandleGameResume;
    }

    private void HandleGameOver()
    {
        // Düşme animasyonunu tetikle
        animator.SetBool("isGameOver", true);

        // Smooth geri tepme hareketini başlat ve ardından yere düşme hareketini başlat
        StartCoroutine(SmoothMoveBackAndFall());
    }

    private float GetGroundY()
    {
        Collider playerCollider = GetComponentInParent<Collider>(); // Parent'taki Collider'ı al
        if (playerCollider != null)
        {
            // Parent objenin alt noktasını al
            float playerBottomY = playerCollider.bounds.min.y;

            // Raycast ile yere olan mesafeyi hesapla
            if (Physics.Raycast(new Vector3(transform.position.x, playerBottomY + 0.1f, transform.position.z), Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                return hit.point.y; // Yere çarpılan noktanın Y pozisyonu
            }
        }

        Debug.LogWarning("Ground not found! Returning default Y position.");
        return 0f; // Varsayılan Y pozisyonu
    }

    private IEnumerator SmoothMoveBackAndFall()
    {
        // Geriye hareket
        yield return SmoothMoveBack();

        // Yere düşme pozisyonunu hesapla
        float groundY = GetGroundY();

        // Yere düşme hareketi
        yield return SmoothFallAndMoveBack(groundY);
    }

    private IEnumerator SmoothMoveBack()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition;
        targetPosition.z -= 1.5f; // Z ekseninde geriye kaydır (değeri ihtiyaca göre ayarlayın)

        float duration = 0.5f; // Hareketin süresi (saniye)
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Hedef pozisyona tam olarak yerleştir
        transform.position = targetPosition;
    }

    private IEnumerator SmoothFallAndMoveBack(float groundY)
    {
        Vector3 startPosition = transform.position;
        Vector3 fallTargetPosition = new Vector3(startPosition.x, groundY, startPosition.z);

        float fallDuration = 0.5f; // Düşme süresi
        float elapsedTime = 0f;

        // Smooth düşme hareketi
        while (elapsedTime < fallDuration)
        {
            transform.position = Vector3.Lerp(startPosition, fallTargetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Hedef düşme pozisyonuna tam olarak yerleştir
        transform.position = fallTargetPosition;
    }

    private void HandleGamePause()
    {
        isPaused = true;
        // Optionally you can also set the animator speed to 0 to freeze the animation completely
        animator.speed = 0;
    }

    private void HandleGameResume()
    {
        isPaused = false;
        // Restore animator speed
        animator.speed = 1;
    }
}
