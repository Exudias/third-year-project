using UnityEngine;

public class NextLevelTrigger : Trigger
{
    [SerializeField] private bool affectSpirit = false;

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;

        if (!collisionIsPlayer) return;

        PlayerFormSwitcher formSwitcher = activator.GetComponent<PlayerFormSwitcher>();

        if (!affectSpirit)
        {
            if (formSwitcher == null) return;
            if (formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Spirit) return;
        }

        GameManager.LoadNextScene(); 
    }
}
