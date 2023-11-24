using UnityEngine;

public class ForceCameraPositionTrigger : Trigger
{
    [SerializeField] private Vector2 forcedPosition;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEntered;
        OnTriggerExit += OnExited;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEntered;
        OnTriggerExit -= OnExited;
    }

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
    }

    private void OnEntered(Collider2D activator)
    {
        Camera.main.GetComponent<CameraManager>().SetForcedCameraPosition(forcedPosition, lockX, lockY);
    }

    private void OnExited(Collider2D activator)
    {
        Camera.main.GetComponent<CameraManager>().DisableForcedCameraPosition();
    }
}
