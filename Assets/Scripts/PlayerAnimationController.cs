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
        animator.SetBool("isGameOver", true);
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
