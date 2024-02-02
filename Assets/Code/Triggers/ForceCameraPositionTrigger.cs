using UnityEngine;

public class ForceCameraPositionTrigger : Trigger
{
    [SerializeField] private Vector2 forcedPosition;
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool instant = false;

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

    const float TIME_FOR_INSTANT = 0.1f;

    private void OnEntered(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            bool isInstant = GameManager.GetSceneTime() < TIME_FOR_INSTANT && instant;
            Camera.main.GetComponent<CameraManager>().SetForcedCameraPosition(forcedPosition, lockX, lockY, isInstant, gameObject.GetInstanceID());
        }
    }

    private void OnExited(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            Camera.main.GetComponent<CameraManager>().DisableForcedCameraPosition(gameObject.GetInstanceID());
        }
    }
}
