using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D playerLight;
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [Header("Settings")]
    [SerializeField] private float minLightMultiplier = 0.2f;
    [SerializeField] private Vector2 bulbLightPosOffset = new Vector2(0, 0.25f);
    [SerializeField] private Vector2 spiritLightPosOffset = new Vector2(0, 0);

    private float startInnerRadius;
    private float startOuterRadius;

    void Start()
    {
        startInnerRadius = playerLight.pointLightInnerRadius;
        startOuterRadius = playerLight.pointLightOuterRadius;
    }

    void Update()
    {
        float energyPercent = energyManager != null && energyManager.enabled ? energyManager.GetEnergyPercent() : 1f;
        playerLight.pointLightInnerRadius = Mathf.Lerp(startInnerRadius * minLightMultiplier, startInnerRadius, energyPercent);
        playerLight.pointLightOuterRadius = Mathf.Lerp(startOuterRadius * minLightMultiplier, startOuterRadius, energyPercent);

        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();
        if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            transform.localPosition = bulbLightPosOffset;
        }
        else
        {
            transform.localPosition = spiritLightPosOffset;
        }
    }
}
