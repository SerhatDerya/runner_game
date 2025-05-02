using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    private IPlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<IPlayerMovement>();
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += playerMovement.Stop;
        GameManager.OnGamePause += playerMovement.Pause;
        GameManager.OnGameResume += playerMovement.Resume;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= playerMovement.Stop;
        GameManager.OnGamePause -= playerMovement.Pause;
        GameManager.OnGameResume -= playerMovement.Resume;
    }
}