using UnityEngine;

public class ChangeRespawnTrigger : Trigger
{
    [SerializeField] private Transform newSpawnTransform;

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnEntered;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnEntered;
    }

    private void OnEntered(Collider2D activator)
    {
        GameManager.SetSpawn(newSpawnTransform.position);
    }
}
