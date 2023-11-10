using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerFormSwitcher))]
public class EnergyManager : MonoBehaviour
{
    private const float MIN_ENERGY = 0;
    private const float MAX_ENERGY = 100;

    [SerializeField] private float startEnergy = 0;
    [SerializeField] private Image energyMeterFG;
    [SerializeField] private float timescaleAtMaxEnergy = 0.1f;
    [SerializeField] private float timescaleAtMinEnergy = 1f;

    private static float energy;

    private PlayerFormSwitcher formSwitcher;
    private SpiritMovement spiritMovement;

    private void OnEnable()
    {
        Controller2D.OnOtherCollision += OnControllerOtherCollision;
    }

    private void OnDisable()
    {
        Controller2D.OnOtherCollision -= OnControllerOtherCollision;
    }

    private void OnControllerOtherCollision(Vector2 dir, GameObject obj)
    {
        if (obj.GetComponent<EnergyPickupLogic>() != null)
        {
            if (spiritMovement != null && spiritMovement.enabled)
            {
                obj.GetComponent<EnergyPickupLogic>().PlayerCollect(this);
            }
        }
    }

    private void Start()
    {
        energy = Mathf.Clamp(startEnergy, MIN_ENERGY, MAX_ENERGY);
        formSwitcher = GetComponent<PlayerFormSwitcher>();
        spiritMovement = GetComponent<SpiritMovement>();
    }

    private void Update()
    {
        if (energyMeterFG != null)
        {
            energyMeterFG.fillAmount = GetEnergyPercent();
        }
        
        if (formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            Time.timeScale = Mathf.Lerp(timescaleAtMinEnergy, timescaleAtMaxEnergy, GetEnergyPercent());
        }
        else
        {
            Time.timeScale = 1;
        }
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
