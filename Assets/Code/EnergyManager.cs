using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    private const float MIN_ENERGY = 0;
    private const float MAX_ENERGY = 100;

    [SerializeField]
    private float startEnergy = 0;
    [SerializeField]
    private Image energyMeterFG;

    public static EnergyManager Instance;

    private static float energy;

    private void Start()
    {
        Instance = this;
        energy = Mathf.Clamp(startEnergy, MIN_ENERGY, MAX_ENERGY);
    }

    private void Update()
    {
        energyMeterFG.fillAmount = GetEnergyPercent();
    }

    public void AddEnergy(float amount)
    {
        energy = Mathf.Clamp(energy + amount, MIN_ENERGY, MAX_ENERGY);
    }

    public float GetEnergy()
    {
        return energy;
    }

    public void ResetEnergy()
    {
        energy = startEnergy;
    }

    public float GetEnergyPercent()
    {
        return (energy - MIN_ENERGY) / (MAX_ENERGY - MIN_ENERGY);
    }
}
