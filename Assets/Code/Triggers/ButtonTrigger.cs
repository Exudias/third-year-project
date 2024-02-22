using UnityEngine;

public class ButtonTrigger : Trigger
{
    [SerializeField] private ButtonVisualsManager buttonVisuals;
    [SerializeField] private ButtonLogic buttonLogic;

    private int pressing = 0;
    private int lastPressing = 0;

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
        pressing++;
    }

    private void OnButtonUnpressed(Collider2D activator)
    {
        pressing--;
    }

    private void Update()
    {
        if (lastPressing != pressing)
        {
            if (lastPressing == 0)
            {
                buttonVisuals.OnButtonPressed();
                buttonLogic.OnButtonPressed();
            }
            else if (pressing == 0)
            {
                buttonVisuals.OnButtonUnpressed();
                buttonLogic.OnButtonUnpressed();
            }
            lastPressing = pressing;
        }
        //Debug.Log(lastPressing + "<- Last | Current ->" + pressing);
    }
}
