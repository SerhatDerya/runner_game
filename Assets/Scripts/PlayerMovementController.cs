using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float laneChangeSpeed = 15f;
    public float jumpForce = 7f;
    public float gravity = 20f;
    public float fallGravityMultiplier = 2f;
    public LayerMask groundLayer;

    private Vector3 targetPosition;
    private CharacterController characterController;
    private Rigidbody rb;
    private int currentLaneIndex;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isChangingLane = false;

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
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        HandleLaneChange();
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.2f, groundLayer);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Hafif bir aşağı ivme (hızlı yere yapışmayı önler)
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
        }

        // **Düşüş sırasında yerçekimini artır (daha doğal bir his için)**
        if (!isGrounded && verticalVelocity < 0)
        {
            verticalVelocity -= gravity * fallGravityMultiplier * Time.deltaTime;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 moveDirection = Vector3.forward * forwardSpeed + Vector3.up * verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
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
