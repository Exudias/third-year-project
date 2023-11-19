using UnityEngine;

public class BulbSpawnerLogic : MonoBehaviour
{
    [SerializeField] private BulbSpawnerVisuals visuals;
    [SerializeField] private Transform playerSpawnPoint;

    public Vector2 GetSpawnPoint() => playerSpawnPoint.position;

    public void HandlePickup()
    {
        visuals.MakeEmpty();
    }
}
