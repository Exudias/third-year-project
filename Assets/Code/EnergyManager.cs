using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    private const float MIN_ENERGY = 0;
    private const float MAX_ENERGY = 100;

    [SerializeField]
    private float startEnergy = 0;

    public static EnergyManager Instance;

    private static float energy;

    private void Start()
    {
        Instance = this;
        energy = Mathf.Clamp(startEnergy, MIN_ENERGY, MAX_ENERGY);
    }

    public void addEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, MIN_ENERGY, MAX_ENERGY);
    }

    public float getEnergy()
    {
        return energy;
    }

    public void resetEnergy()
    {
        energy = startEnergy;
    }

    public float getEnergyPercent()
    {
        return (energy - MIN_ENERGY) / (MAX_ENERGY - MIN_ENERGY);
    }
}
