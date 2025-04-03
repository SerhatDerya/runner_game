using UnityEngine;
 
public class PlayerMovementController : MonoBehaviour
{
    public float forwardSpeed = 15f;
    public float laneChangeSpeed = 15f;
    public float jumpForce = 9.8f;
    public float gravity = 9.8f;
    public float fallGravityMultiplier = 2.5f;
    public LayerMask groundLayerMask;
    private bool jumpRequested;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private int currentLaneIndex;
    private float verticalVelocity = 0f;
    private bool isGrounded;
    private bool isChangingLane = false;
    private Vector3 lastScoredPosition;
    private float distanceForScore = 1f; // Add 1 point per meter
    private void OnEnable()
    {
        GameManager.OnLaneChange += UpdateLane;
    }
 
    private void OnDisable()
    {
        GameManager.OnLaneChange -= UpdateLane;
    }
 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastScoredPosition = transform.position;
    }
 
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }
        HandleLaneChange();
        
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
    private void Move()
    {
        float height = GetComponent<Collider>().bounds.size.y;
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
 
 
    private void HandleLaneChange()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
 
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