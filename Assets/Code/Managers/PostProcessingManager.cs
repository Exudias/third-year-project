using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private float vignetteIntensityBulb = 0.1f;
    [SerializeField] private float vignetteIntensitySpirit = 0.4f;
    [SerializeField] [ColorUsage(false, true)] private Color colorFilterBulb;
    [SerializeField] [ColorUsage(false, true)] private Color colorFilterSpirit;

    private VolumeProfile profile;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    private PlayerFormSwitcher.PlayerForm playerForm;

    private void OnEnable()
    {
        PlayerFormSwitcher.OnInitAsBulb += ChangeToBulb;
        PlayerFormSwitcher.OnSwitchToBulb += ChangeToBulb;
        PlayerFormSwitcher.OnInitAsSpirit += ChangeToSpirit;
        PlayerFormSwitcher.OnSwitchToSpirit += ChangeToSpirit;
    }

    private void OnDisable()
    {
        PlayerFormSwitcher.OnInitAsBulb -= ChangeToBulb;
        PlayerFormSwitcher.OnSwitchToBulb -= ChangeToBulb;
        PlayerFormSwitcher.OnInitAsSpirit -= ChangeToSpirit;
        PlayerFormSwitcher.OnSwitchToSpirit -= ChangeToSpirit;
    }

    private void ChangeToBulb()
    {
        playerForm = PlayerFormSwitcher.PlayerForm.Bulb;
    }

    private void ChangeToSpirit()
    {
        playerForm = PlayerFormSwitcher.PlayerForm.Spirit;
    }

    void Start()
    {
        profile = GetComponent<Volume>().profile;
        profile.TryGet(out vignette);
        profile.TryGet(out colorAdjustments);
    }

    void Update()
    {
        float targetVignetteIntensity;
        Color targetColorFilter;

        if (playerForm == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            targetVignetteIntensity = vignetteIntensitySpirit;
            targetColorFilter = colorFilterSpirit;
        }
        else
        {
            targetVignetteIntensity = vignetteIntensityBulb;
            targetColorFilter = colorFilterBulb;
        }

        vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, targetVignetteIntensity, 1.5f * Time.unscaledDeltaTime);
        colorAdjustments.colorFilter.value = Vector4.MoveTowards(colorAdjustments.colorFilter.value, targetColorFilter, 2f * Time.unscaledDeltaTime);
    }
}
