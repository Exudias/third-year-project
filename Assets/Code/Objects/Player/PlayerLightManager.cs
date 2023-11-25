using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightManager : MonoBehaviour
{
    [SerializeField] private Light2D playerLight;
    [SerializeField] private float minLightMultiplier = 0.2f;
    [SerializeField] private EnergyManager energyManager;

    private float startInnerRadius;
    private float startOuterRadius;

    void Start()
    {
        startInnerRadius = playerLight.pointLightInnerRadius;
        startOuterRadius = playerLight.pointLightOuterRadius;
    }

    void Update()
    {
        float energyPercent = energyManager != null ? energyManager.GetEnergyPercent() : 1f;
        playerLight.pointLightInnerRadius = Mathf.Lerp(startInnerRadius * minLightMultiplier, startInnerRadius, energyPercent);
        playerLight.pointLightOuterRadius = Mathf.Lerp(startOuterRadius * minLightMultiplier, startOuterRadius, energyPercent);
    }
}
