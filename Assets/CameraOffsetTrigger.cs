using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffsetTrigger : MonoBehaviour
{
    [SerializeField] private bool affectBulb;
    [SerializeField] private bool affectSpirit;
    [SerializeField] private Vector2 offsetToApply;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerFormSwitcher formSwitcher = collision.GetComponent<PlayerFormSwitcher>();

        if (formSwitcher != null)
        {
            bool successfullyAffectBulb = affectBulb && formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Bulb;
            bool successfullyAffectSpirit = affectSpirit && formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit;

            bool successfullyAffect = successfullyAffectBulb || successfullyAffectSpirit;

            if (successfullyAffect)
            {
                Camera.main.GetComponent<CameraManager>().SetCurrentCameraOffset(offsetToApply);
            }
        }
    }
}
