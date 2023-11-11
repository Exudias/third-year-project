using UnityEngine;

public class EnergyPickupLogic : MonoBehaviour
{
    [SerializeField] private float energyValue = 10;

    public void PlayerCollect(EnergyManager energyManager)
    {
        energyManager?.AddEnergy(energyValue);
        Destroy(gameObject);
    }
}
