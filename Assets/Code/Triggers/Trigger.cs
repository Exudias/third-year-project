using UnityEngine;

public class Trigger : MonoBehaviour
{
    private bool wasTriggeredLastFrame;
    private bool triggeredThisFrame;

    private Collider2D lastActivator;

    public delegate void TriggerEvent(Collider2D activator);
    public event TriggerEvent OnTriggerEnter;
    public event TriggerEvent OnTriggerStay;
    public event TriggerEvent OnTriggerExit;

    public virtual void OnEnable()
    {
        Controller2D.OnPostControllerMove += TriggerUpdate;
    }

    public virtual void OnDisable()
    {
        Controller2D.OnPostControllerMove -= TriggerUpdate;
    }

    private void Start()
    {
        wasTriggeredLastFrame = false;
        triggeredThisFrame = false;
    }

    public virtual void Activate(Collider2D activator)
    {
        triggeredThisFrame = true;
        lastActivator = activator;

        OnTriggerStay?.Invoke(activator);
    }

    private void TriggerUpdate()
    {
        if (!wasTriggeredLastFrame && triggeredThisFrame)
        {
            OnTriggerEnter?.Invoke(lastActivator);
        }
        if (wasTriggeredLastFrame && !triggeredThisFrame)
        {
            OnTriggerExit?.Invoke(lastActivator);
        }
        wasTriggeredLastFrame = triggeredThisFrame;
        triggeredThisFrame = false;
    }
}
