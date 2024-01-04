using UnityEngine;

public class SpiritGateLogic : MonoBehaviour
{
    [SerializeField] private GameObject gate;

    private void OnEnable()
    {
        PlayerFormSwitcher.OnSwitchToSpirit += OnSpirit;
        PlayerFormSwitcher.OnInitAsSpirit += OnSpirit;
        PlayerFormSwitcher.OnSwitchToBulb += OnBulb;
        PlayerFormSwitcher.OnInitAsBulb += OnBulb;
    }

    private void OnDisable()
    {
        PlayerFormSwitcher.OnSwitchToSpirit -= OnSpirit;
        PlayerFormSwitcher.OnInitAsSpirit -= OnSpirit;
        PlayerFormSwitcher.OnSwitchToBulb -= OnBulb;
        PlayerFormSwitcher.OnInitAsBulb -= OnBulb;
    }

    private void OnSpirit()
    {
        gate.SetActive(true);
    }

    private void OnBulb()
    {
        gate.SetActive(false);
    }
}
