using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementController playerMovement;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovementController>(); // Parent'taki hareket scriptini al
    }

    private void Update()
    {
        if (playerMovement == null) return;

        // Koşma Animasyonu
        animator.SetFloat("speed", playerMovement.forwardSpeed);

        // Zıplama Animasyonu
        animator.SetBool("isJumping", !playerMovement.isGrounded);

    }
}
