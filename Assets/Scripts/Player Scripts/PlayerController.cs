using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerMovement movement;
    private ScoreTracker scoreTracker;
    private AudioManager audioManager;
    private PlayerAnimationController animationController;
    private ColliderSwitcher colliderSwitcher;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float swipeThreshold = 50f;
    private Vector3 lastScoredPosition;
    private float distanceForScore = 1f;

    private void Awake()
    {
        movement = GetComponentInChildren<IPlayerMovement>();
        animationController = GetComponentInChildren<PlayerAnimationController>();
        colliderSwitcher = GetComponentInChildren<ColliderSwitcher>();
        scoreTracker = GetComponentInChildren<ScoreTracker>();
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGamePause += OnGamePause;
        GameManager.OnGameResume += OnGameResume;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= OnGameOver;
        GameManager.OnGamePause -= OnGamePause;
        GameManager.OnGameResume -= OnGameResume;
    }

    public void OnGameOver()
    {
        movement?.Stop();
        animationController?.HandleGameOver();
    }
    private void OnGamePause() => animationController?.HandleGamePause();
    private void OnGameResume() => animationController?.HandleGameResume();

    private void Start()
    {
        colliderSwitcher?.SwitchToStanding();
        lastScoredPosition = transform.position;
        StartCoroutine(StartDelay(2f));
        scoreTracker?.Init(transform.position);
    }

    private IEnumerator StartDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        movement?.StartMoving();
        GameManager.OnPlayerStartedRunning?.Invoke();
    }

    private void Update()
    {
        int horizontalInput = 0;

        // mobil touch input kontrolü
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                Vector2 swipe = touchEndPos - touchStartPos;

                if (swipe.magnitude > swipeThreshold)
                {
                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                    {
                        horizontalInput = swipe.x > 0 ? 1 : -1;
                    }
                    else if (swipe.y > 0 && movement?.IsGrounded() == true)
                    {
                        movement?.Jump();
                        audioManager?.PlaySFX(audioManager.jump);
                    }
                }
            }
        }

        // pc klavye input kontrolü
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space) && movement?.IsGrounded() == true)
        {
            movement?.Jump();
            audioManager?.PlaySFX(audioManager.jump);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            horizontalInput = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            horizontalInput = 1;
#endif

        if (Vector3.Distance(transform.position, lastScoredPosition) >= distanceForScore)
        {
            scoreTracker?.TrackDistance(transform.position);
            lastScoredPosition = transform.position;
        }

        movement?.Move(horizontalInput);
    }

    private void FixedUpdate()
    {
        movement?.UpdateMovement();
    }

    public void SwitchToFallenCollider()
    {
        colliderSwitcher?.SwitchToFallen();
    }

    public void ResetState()
    {
        movement?.Stop();
        colliderSwitcher?.SwitchToStanding();
        transform.position = Vector3.zero;
        StartCoroutine(StartDelay(2f));
    }
}