using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float forward_speed = 5f;
    public float laneChangeSpeed = 15f;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private bool isChangingLane = false;
    private int currentLaneIndex;

    private void OnEnable()
    {
        GameManager.OnLaneChange += UpdateLane; // Event'e abone ol
    }

    private void OnDisable()
    {
        GameManager.OnLaneChange -= UpdateLane; // Event'ten çık
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        MoveForward();
        HandleLaneChange();
    }

    void MoveForward()
    {
        //transform üzerinden hareket
        transform.Translate(Vector3.forward * forward_speed * Time.deltaTime);
    }

    void HandleLaneChange()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D ve Sol/Sağ

        if (!isChangingLane)
        {
            if (horizontalInput > 0 && currentLaneIndex < LaneManager.instance.laneCount - 1) // Sağ şeride geç
            {
                currentLaneIndex++;
                isChangingLane = true;
            }
            else if (horizontalInput < 0 && currentLaneIndex > 0) // Sol şeride geç
            {
                currentLaneIndex--;
                isChangingLane = true;
            }
        }

        if (horizontalInput == 0)
        {
            isChangingLane = false;
        }

        // Yeni şerit pozisyonunu LaneManager'dan al
        targetPosition = new Vector3(LaneManager.instance.GetLanePosition(currentLaneIndex), transform.position.y, transform.position.z);

        // Sağa-sola yumuşak geçiş
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneChangeSpeed);
    }
}