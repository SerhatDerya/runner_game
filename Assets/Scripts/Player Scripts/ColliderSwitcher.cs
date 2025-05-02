using UnityEngine;

public class ColliderSwitcher : MonoBehaviour
{
    public CapsuleCollider standingCollider;
    public BoxCollider fallenCollider;

    public void SwitchToFallen()
    {
        standingCollider.enabled = false;
        fallenCollider.enabled = true;
    }

    public void SwitchToStanding()
    {
        standingCollider.enabled = true;
        fallenCollider.enabled = false;
    }
}