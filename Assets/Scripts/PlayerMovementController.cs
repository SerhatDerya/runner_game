using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    AudioManager audioManager;
    public float forwardSpeed = 15f;
    private float originalForwardSpeed = 15f; // Store the original speed
    public float laneChangeSpeed = 15f;
    public float jumpForce = 9.8f;
    public float gravity = 9.8f;
    public float fallGravityMultiplier = 2.5f;
    public CapsuleCollider standingCollider;
    public BoxCollider fallenCollider;
    public LayerMask groundLayerMask;
    private bool jumpRequested;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private int currentLaneIndex;
    private float verticalVelocity = 0f;
    public bool isGrounded { get; private set; } // Değişken dışarıdan sadece okunabilir

    private bool isChangingLane = false;
    private Vector3 lastScoredPosition;
    private float distanceForScore = 1f; // Add 1 point per meter
    public float startDelay = 2f; // Kaç saniye bekleyeceğini belirle
    private bool canMove = false;
    private bool isPaused = false; // Track if the game is paused
    
    private void OnEnable()
    {
        GameManager.OnLaneChange += UpdateLane;
        GameManager.OnGameOver += StopMovement;
        GameManager.OnGamePause += PauseMovement; // New subscription for pause event
        GameManager.OnGameResume += ResumeMovement; // New subscription for resume event
    }

    private void OnDisable()
    {
        GameManager.OnLaneChange -= UpdateLane;
        GameManager.OnGameOver -= StopMovement;
        GameManager.OnGamePause -= PauseMovement; // Clean up pause event subscription
        GameManager.OnGameResume -= ResumeMovement; // Clean up resume event subscription
    }
    
    private void StopMovement()
    {
        forwardSpeed = 0f;
        jumpRequested = false;
    }
    
    // New method to pause movement
    private void PauseMovement()
    {
        if (!isPaused)
        {
            isPaused = true;
            originalForwardSpeed = forwardSpeed;
            forwardSpeed = 0f;
            jumpRequested = false;
        }
    }
    
    // New method to resume movement
    private void ResumeMovement()
    {
        if (isPaused)
        {
            isPaused = false;
            forwardSpeed = originalForwardSpeed;
        }
    }

    public void SwitchToFallenCollider()
    {
        standingCollider.enabled = false;
        fallenCollider.enabled = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastScoredPosition = transform.position;
        originalForwardSpeed = forwardSpeed; // Store the initial speed
        StartCoroutine(StartDelay());
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (isPaused) return; // Skip input handling if game is paused
        
        int horizontalInput = 0;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            audioManager.PlaySFX(audioManager.jump);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            horizontalInput = -1;
            audioManager.PlaySFX(audioManager.swipe);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            horizontalInput = 1;
            audioManager.PlaySFX(audioManager.swipe);
        }
        else
        {
            horizontalInput = 0;
        }
        HandleLaneChange(horizontalInput);

        float distanceTraveled = transform.position.z - lastScoredPosition.z;
        if (distanceTraveled >= distanceForScore && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScoreForDistance(distanceTraveled);
            lastScoredPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    private IEnumerator StartDelay()
    {
        forwardSpeed = 0; // Başlangıçta hızı 0 yap
        yield return new WaitForSeconds(startDelay); // Belirtilen süre kadar bekle
        forwardSpeed = originalForwardSpeed; // Sonra normal hızına getir
        canMove = true; // Hareket etmeye başlasın
    }

    private void Move()
    {
        if (!canMove || isPaused) return; // Skip movement if game is not ready or is paused

        float height = GetCurrentHeight();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (height / 2) + 0.1f, groundLayerMask);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f; // Yere çarptığında hızı sıfırla

        }

        if (isGrounded && jumpRequested)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            jumpRequested = false;
        }

        // Yerçekimi uygula
        if (!isGrounded)
        {
            verticalVelocity -= gravity * fallGravityMultiplier * Time.deltaTime;
        }

        // Hareket
        Vector3 moveDirection = new Vector3(0, verticalVelocity, forwardSpeed);
        transform.position += moveDirection * Time.deltaTime;

        // Zemin Kontrolü
        if (isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, (height / 2) + 0.2f, groundLayerMask))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + (height / 2), transform.position.z);
            }
        }
    }

    private float GetCurrentHeight()
    {
        if (standingCollider.enabled)
            return standingCollider.bounds.size.y;
        else if (fallenCollider.enabled)
            return fallenCollider.bounds.size.y;
        else
            return 1f; // Varsayılan bir değer
    }

    private void HandleLaneChange(int horizontalInput)
    {
        if (isPaused) return; // Skip lane change if game is paused

        if (!isChangingLane) // Sadece yeni bir input olduğunda çalışır
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

        // Eğer hiç input yapılmadıysa tekrar şerit değiştirilebilir
        if (horizontalInput == 0)
        {
            isChangingLane = false;
        }

        targetPosition = new Vector3(
            LaneManager.instance.GetLanePosition(currentLaneIndex),
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneChangeSpeed);
    }

    private void UpdateLane(int newLaneIndex)
    {
        currentLaneIndex = newLaneIndex;
        transform.position = new Vector3(
            LaneManager.instance.GetLanePosition(currentLaneIndex),
            transform.position.y,
            transform.position.z
        );
    }
}