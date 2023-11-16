using UnityEngine;

public class ButtonTrigger : Trigger
{
    [SerializeField] private ButtonVisualsManager buttonVisuals;
    [SerializeField] private ButtonLogic buttonLogic;

    public override void OnEnable()
    {
        base.OnEnable();
        OnTriggerEnter += OnButtonPressed;
        OnTriggerExit += OnButtonUnpressed;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnTriggerEnter -= OnButtonPressed;
        OnTriggerExit -= OnButtonUnpressed;
    }

    public override void Activate(Collider2D activator)
    {
        base.Activate(activator);
    }

    private void OnButtonPressed(Collider2D activator)
    {
        buttonVisuals.OnButtonPressed();
        buttonLogic.OnButtonPressed();
    }

    private void OnButtonUnpressed(Collider2D activator)
    {
        buttonVisuals.OnButtonUnpressed();
        buttonLogic.OnButtonUnpressed();
    }
}
