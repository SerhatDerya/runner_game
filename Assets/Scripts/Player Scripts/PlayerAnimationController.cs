using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private IPlayerMovement playerMovement;
    private bool isPaused = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<IPlayerMovement>();
    }

    private void Update()
    {
        if (isPaused || playerMovement == null) return;

        // Koşma animasyonu (speed parametresi)
        animator.SetFloat("speed", playerMovement.GetForwardSpeed());

        // Zıplama bool animasyonu (havadaysa true)
        animator.SetBool("isJumping", !playerMovement.IsGrounded());
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

    public void HandleGameOver()
    {
        if (animator == null || !animator.isActiveAndEnabled) return;

        animator.SetBool("isFalling", true);
        StartCoroutine(SmoothMoveBackAndFall());
    }

    public void HandleGamePause()
    {
        isPaused = true;
        animator.speed = 0f;
    }

    public void HandleGameResume()
    {
        isPaused = false;
        animator.speed = 1f;
    }

    private IEnumerator SmoothMoveBackAndFall()
    {
        yield return SmoothMoveBack();

        float groundY = GetGroundY();
        yield return SmoothFallAndMoveBack(groundY);
    }

    private IEnumerator SmoothMoveBack()
    {
        Vector3 start = transform.position;
        Vector3 target = start + Vector3.back * 1.5f;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator SmoothFallAndMoveBack(float groundY)
    {
        Vector3 start = transform.position;
        Vector3 target = new Vector3(start.x, groundY, start.z);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private float GetGroundY()
    {
        Collider playerCollider = GetComponentInParent<Collider>();
        if (playerCollider != null)
        {
            float bottomY = playerCollider.bounds.min.y;
            if (Physics.Raycast(new Vector3(transform.position.x, bottomY + 0.1f, transform.position.z), Vector3.down, out RaycastHit hit))
            {
                return hit.point.y;
            }
        }

        Debug.LogWarning("Ground not found! Using fallback Y.");
        return transform.position.y - 1f;
    }

    public void UpdateRunAnimation(float speed)
    {
        animator.SetFloat("speed", speed);
    }

    public void UpdateJumpState(bool isJumping)
    {
        animator.SetBool("isJumping", isJumping);
    }
}