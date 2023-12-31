using UnityEngine;

public class CameraOffsetTrigger : Trigger
{
    [SerializeField] private bool affectBulb;
    [SerializeField] private bool affectSpirit;
    [SerializeField] private Vector2 offsetToApply;

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);

        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (!collisionIsPlayer) return;

        PlayerFormSwitcher formSwitcher = activator.GetComponent<PlayerFormSwitcher>();

        if (formSwitcher != null)
        {
            bool successfullyAffectBulb = affectBulb && formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Bulb;
            bool successfullyAffectSpirit = affectSpirit && formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit;

            bool successfullyAffect = successfullyAffectBulb || successfullyAffectSpirit;

            if (successfullyAffect)
            {
                Camera.main.GetComponent<CameraManager>().SetCurrentCameraOffset(offsetToApply, affectBulb, affectSpirit);
            }
        }
    }
}
