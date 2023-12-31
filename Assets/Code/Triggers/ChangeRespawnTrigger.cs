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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(newSpawnTransform.position + new Vector3(0, -0.03125f, 0), new Vector2(0.375f, 0.9375f));
    }
}
