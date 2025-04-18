public interface IPlayerMovement
{
    void Move(float horizontalInput);
    void Jump();
    void UpdateMovement();
    void Stop();
    void Pause();
    void Resume();
    void StartMoving();
    float GetForwardSpeed();
    bool IsGrounded();
    float GetVerticalVelocity();
}