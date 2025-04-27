using UnityEngine;

public class PlayerMover : MonoBehaviour, IPlayerMovement
{
    [Header("Speed Progression Settings")]
    [SerializeField] private float speedIncreaseInterval = 100f; // her 100 puanda hız artışı
    [SerializeField] private float speedIncreaseAmount = 0.5f;  // hız artış miktarı
    [SerializeField] private float maxForwardSpeed = 30f;       // maksimum ileri hız
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float defaultForwardSpeed = 15f;
    [SerializeField] private float laneChangeSpeed = 15f;
    [SerializeField] private float jumpForce = 9.8f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private CapsuleCollider standingCollider;
    [SerializeField] private BoxCollider fallenCollider;
    [SerializeField] private LayerMask groundLayerMask;

    private float verticalVelocity = 0f;
    private bool jumpRequested = false;
    private bool isPaused = false;
    private bool canMove = false;
    private bool isGrounded;
    private Vector3 targetPosition;
    private int currentLaneIndex;
    private bool isChangingLane = false;

    private void OnEnable()
    {
        ScoreTracker.OnScoreChanged += UpdateSpeedBasedOnScore;
    }

    private void OnDisable()
    {
        ScoreTracker.OnScoreChanged -= UpdateSpeedBasedOnScore;
    }

    private void Start()
    {
        targetPosition = transform.position;
        currentLaneIndex = LaneManager.instance.GetMiddleLaneIndex();

        // başlangıçta hareketi dondur
        forwardSpeed = 0f;
    }

    public void Jump()
    {
        if (!canMove || isPaused || !isGrounded) return;
        jumpRequested = true;
    }

    public void Move(float horizontalInput)
    {
        if (isPaused) return;

        if (!isChangingLane)
        {
            if (horizontalInput > 0 && currentLaneIndex < LaneManager.instance.laneCount - 1)
            {
                currentLaneIndex++;
                isChangingLane = true;
            }
            else if (horizontalInput < 0 && currentLaneIndex > 0)
            {
                currentLaneIndex--;
                isChangingLane = true;
            }
        }

        if (horizontalInput == 0)
        {
            isChangingLane = false;
        }

        targetPosition = new Vector3(
            LaneManager.instance.GetLanePosition(currentLaneIndex),
            transform.position.y,
            transform.position.z
        );
    }

    public void UpdateMovement()
    {
        if (!canMove || isPaused) return;

        float height = GetCurrentHeight();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (height / 2) + 0.1f, groundLayerMask);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }

        if (jumpRequested && isGrounded)
        {
            verticalVelocity = jumpForce;
            jumpRequested = false;
            isGrounded = false;
        }

        if (!isGrounded)
        {
            verticalVelocity -= gravity * fallGravityMultiplier * Time.fixedDeltaTime;
        }

        // Y hareketi ve Z ekseni boyunca sabit ileri
        Vector3 moveDirection = new Vector3(0f, verticalVelocity, forwardSpeed);
        transform.position += moveDirection * Time.fixedDeltaTime;

        // Lerp ile x geçişi
        float newX = Mathf.Lerp(transform.position.x, targetPosition.x, Time.fixedDeltaTime * laneChangeSpeed);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, (height / 2) + 0.1f, groundLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * ((height / 2) + 0.1f), Color.red);
    }

    public void Stop()
    {
        forwardSpeed = 0f;
        jumpRequested = false;
        isChangingLane = false;
        canMove = false;
        // targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void StartMoving()
    {
        forwardSpeed = defaultForwardSpeed; // Editörde ayarlanan hız
        canMove = true;
    }

    public void UpdateSpeedBasedOnScore(int score)
    {
        if (!canMove) return;

        int intervalsPassed = Mathf.FloorToInt(score / speedIncreaseInterval);
        float targetSpeed = defaultForwardSpeed + (intervalsPassed * speedIncreaseAmount);

        forwardSpeed = Mathf.Min(targetSpeed, maxForwardSpeed);
    }

    private float GetCurrentHeight()
    {
        if (standingCollider.enabled) return standingCollider.bounds.size.y;
        if (fallenCollider.enabled) return fallenCollider.bounds.size.y;
        return 1f;
    }

    public float GetForwardSpeed()
    {
        return forwardSpeed;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }
}