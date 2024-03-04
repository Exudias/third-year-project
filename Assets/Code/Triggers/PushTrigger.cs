using UnityEngine;

public class PushTrigger : Trigger
{
    [SerializeField] private Vector2 velocityToApply = new Vector2(-2, 0);

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnActorEnter;
        OnTriggerExit += OnActorExit;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnActorEnter;
        OnTriggerExit -= OnActorExit;
    }

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
    }

    private void OnActorEnter(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            if (activator.GetComponent<PlayerFormSwitcher>().GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit) return;
            BulbMovement bulbMovement = activator.GetComponent<BulbMovement>();
            bulbMovement.SetExternalVelocity(velocityToApply.x);
        }
        EmptyBulbLogic bulb = activator.transform.GetComponentInChildren<EmptyBulbLogic>();
        if (bulb != null)
        {
            bulb.SetPushSpeed(velocityToApply.x);
        }
    }

    private void OnActorExit(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            if (activator.GetComponent<PlayerFormSwitcher>().GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit) return;
            BulbMovement bulbMovement = activator.GetComponent<BulbMovement>();
            bulbMovement.SetExternalVelocity(0);
        }
        EmptyBulbLogic bulb = activator.transform.GetComponentInChildren<EmptyBulbLogic>();
        if (bulb != null)
        {
            bulb.SetPushSpeed(0);
        }
    }

    public Vector2 GetVelocity() => velocityToApply;
}
