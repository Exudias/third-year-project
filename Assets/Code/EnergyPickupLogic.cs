using UnityEngine;

public class EnergyPickupLogic : MonoBehaviour
{
    [SerializeField] private float energyValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SpiritMovement playerSpirit = collision.GetComponent<SpiritMovement>();

            if (playerSpirit != null && playerSpirit.enabled)
            {
                PlayerCollect(collision.GetComponent<EnergyManager>());
            }
        }
    }

    private void PlayerCollect(EnergyManager energyManager)
    {
        energyManager?.AddEnergy(energyValue);
        Destroy(gameObject);
    }
}
