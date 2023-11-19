using UnityEngine;

public class PlayerDetectorTrigger : Trigger
{
    private bool playerInside;

    private void Start()
    {
        playerInside = false;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEnter;
        OnTriggerExit += OnExit;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEnter;
        OnTriggerExit -= OnExit;
    }

    public bool IsPlayerInside() => playerInside;

    private void OnEnter(Collider2D activator)
    {
        playerInside = true;
    }

    private void OnExit(Collider2D activator)
    {
        playerInside = false;
    }
}
