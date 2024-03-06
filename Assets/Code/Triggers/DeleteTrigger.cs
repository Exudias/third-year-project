using UnityEngine;

public class DeleteTrigger : Trigger
{
    [SerializeField] private GameObject target;

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;

        if (!collisionIsPlayer) return;

        Destroy(target);
    }
}
