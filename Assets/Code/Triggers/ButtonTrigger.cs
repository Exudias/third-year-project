using UnityEngine;

public class ButtonTrigger : Trigger
{
    [SerializeField] private ButtonVisualsManager buttonVisuals;
    [SerializeField] private ButtonLogic buttonLogic;

    public override void Activate(Collider2D activator)
    {
        buttonVisuals.OnButtonPressed();
        buttonLogic.OnButtonPressed();
    }
}
