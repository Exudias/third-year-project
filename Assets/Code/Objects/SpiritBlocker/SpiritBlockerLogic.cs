using UnityEngine;

public class SpiritBlockerLogic : Trigger
{
    [SerializeField] private float dangerFlashTime = 0.2f;
    [SerializeField] private Color dangerFlashColour = Color.red;
    [SerializeField] private SpriteRenderer sprite;

    private bool playerInside;
    private Transform player;

    private float dangerLevel = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEntered;
        OnTriggerExit += OnExited;
        PlayerFormSwitcher.OnFailedSwitchToSpirit += OnPlayerFailTransform;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEntered;
        OnTriggerExit -= OnExited;
        PlayerFormSwitcher.OnFailedSwitchToSpirit -= OnPlayerFailTransform;
    }

    private void Update()
    {
        if (player != null)
        {
            sprite.material.SetVector("_PlayerPosition", new Vector2(player.position.x, player.position.y));
        }

        dangerLevel = Mathf.Clamp01(dangerLevel - Time.unscaledDeltaTime * (1 / dangerFlashTime));

        sprite.material.SetVector("_DangerColour", Color.Lerp(Color.white, dangerFlashColour, dangerLevel));
    }

    private void OnPlayerFailTransform()
    {
        if (playerInside)
        {
            FlashDanger();
        }
    }

    private void FlashDanger()
    {
        dangerLevel = 1;
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
