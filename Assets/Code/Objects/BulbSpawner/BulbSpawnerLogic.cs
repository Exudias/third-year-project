using UnityEngine;

public class BulbSpawnerLogic : MonoBehaviour
{
    [SerializeField] private BulbSpawnerVisuals visuals;
    [SerializeField] private Transform playerSpawnPoint;

    private bool empty;

    public bool IsEmpty() => empty;

    public Vector2 GetSpawnPoint() => playerSpawnPoint.position;

    private void Start()
    {
        empty = false;
    }

    public void HandlePickup()
    {
        visuals.MakeEmpty();
        empty = true;
    }
}
