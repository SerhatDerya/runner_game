using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerMovement movement;
    private ScoreTracker scoreTracker;
    private AudioManager audioManager;
    private PlayerAnimationController animationController;
    private ColliderSwitcher colliderSwitcher;

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
        if (Input.GetKeyDown(KeyCode.Space) && movement?.IsGrounded() == true)
        {
            movement?.Jump(); // sadece flag set eder
            audioManager?.PlaySFX(audioManager.jump);
        }
        if (Vector3.Distance(transform.position, lastScoredPosition) >= distanceForScore)
        {
            scoreTracker?.TrackDistance(transform.position);
            lastScoredPosition = transform.position;
        }

        int horizontalInput = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) horizontalInput = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) horizontalInput = 1;

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