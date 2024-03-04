using UnityEngine;

public class PlayerDetectorTrigger : Trigger
{
    private bool playerInside;

    private bool insideLastFrame;

    public override void Start()
    {
        base.Start();
        playerInside = false;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEnter;
        OnTriggerExit += OnExit;
        OnTriggerStay += OnStay;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEnter;
        OnTriggerExit -= OnExit;
        OnTriggerStay -= OnStay;
    }

    public bool IsPlayerInside() => playerInside;

    private void OnEnter(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;
        playerInside = true;
    }

    private void OnExit(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;
        playerInside = false;
        insideLastFrame = false;
    }

    private void OnStay(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        // Disgusting hack to prevent changing form from affecting insideness
        // Would do better but there are 10 days left and I have priorities!
        if (insideLastFrame)
        {
            playerInside = true;
        }
        insideLastFrame = true;
    }
}
