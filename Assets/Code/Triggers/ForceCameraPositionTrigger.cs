using UnityEngine;

public class ForceCameraPositionTrigger : Trigger
{
    [SerializeField] private Vector2 forcedPosition;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool instant = false;
    [SerializeField] private bool instantOnlyOnce = true;

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
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            Camera.main.GetComponent<CameraManager>().SetForcedCameraPosition(forcedPosition, lockX, lockY, instant);
            if (instant && instantOnlyOnce)
            {
                instant = false;
            }
        }
    }

    private void OnExited(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            Camera.main.GetComponent<CameraManager>().DisableForcedCameraPosition();
        }
    }
}
