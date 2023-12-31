using UnityEngine;

public class SpiritBlockerLogic : Trigger
{
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

    private void OnEntered(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        PlayerFormSwitcher switcher = activator.GetComponent<PlayerFormSwitcher>();
        switcher.SetCanTransform(false);
    }

    private void OnExited(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        PlayerFormSwitcher switcher = activator.GetComponent<PlayerFormSwitcher>();
        switcher.SetCanTransform(true);
    }
}
