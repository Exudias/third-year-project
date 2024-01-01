using UnityEngine;

public class SpiritBlockerLogic : Trigger
{
    [SerializeField] private SpriteRenderer sprite;

    private bool playerInside;
    private Transform player;

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

    private void Update()
    {
        if (player != null)
        {
            sprite.material.SetVector("_PlayerPosition", new Vector2(player.position.x, player.position.y));
        }
    }

    private void OnEntered(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        playerInside = true;
        player = activator.transform;

        PlayerFormSwitcher switcher = activator.GetComponent<PlayerFormSwitcher>();
        switcher.SetCanTransform(false);
    }

    private void OnExited(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        playerInside = false;

        PlayerFormSwitcher switcher = activator.GetComponent<PlayerFormSwitcher>();
        switcher.SetCanTransform(true);
    }
}
