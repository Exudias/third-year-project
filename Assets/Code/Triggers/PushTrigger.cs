using UnityEngine;

public class PushTrigger : Trigger
{
    [SerializeField] private Vector2 velocityToApply = new Vector2(-2, 0);

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerStay += OnActorInside;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerStay -= OnActorInside;
    }

    private void OnActorInside(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            if (activator.GetComponent<PlayerFormSwitcher>().GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit) return;
            BulbMovement bulbMovement = activator.GetComponent<BulbMovement>();
            bulbMovement.SetExternalVelocity(velocityToApply);
        }
    }
}
