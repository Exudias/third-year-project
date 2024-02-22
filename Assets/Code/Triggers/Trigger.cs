using UnityEngine;
using System.Collections.Generic;

public class Trigger : MonoBehaviour
{
    private List<Collider2D> inLastFrame;
    private List<Collider2D> inThisFrame;

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

    public virtual void Start()
    {
        inLastFrame = new List<Collider2D>();
        inThisFrame = new List<Collider2D>();
    }

    public virtual void Activate(Collider2D activator)
    {
        if (!inThisFrame.Contains(activator))
        {
            inThisFrame.Add(activator);
        }

        OnTriggerStay?.Invoke(activator);
    }

    private void TriggerUpdate(Controller2D controller)
    {
        Collider2D activator = controller.GetCurrentCollider();
        if (activator == null) return;

        // CLEAN MISSING THINGS
        List<Collider2D> staticInLastFrame = new List<Collider2D>();

        inLastFrame.ForEach((item) =>
        {
            staticInLastFrame.Add(item);
        });

        foreach (Collider2D coll in staticInLastFrame)
        {
            if (coll == null)
            {
                OnTriggerExit?.Invoke(coll);
                inLastFrame.Remove(coll);
            }
        }

        if (!inLastFrame.Contains(activator) && inThisFrame.Contains(activator))
        {
            OnTriggerEnter?.Invoke(activator);
        }
        if (inLastFrame.Contains(activator) && !inThisFrame.Contains(activator))
        {
            OnTriggerExit?.Invoke(activator);
        }

        if (!inThisFrame.Contains(activator) && inLastFrame.Contains(activator))
        {
            inLastFrame.Remove(activator);    
        }
        if (inThisFrame.Contains(activator) && !inLastFrame.Contains(activator))
        {
            inLastFrame.Add(activator);
        }

        inThisFrame.Remove(activator);
    }

    public virtual void OnDrawGizmos()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.05f);
        Gizmos.DrawCube(bounds.center, bounds.size);
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.1f);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
