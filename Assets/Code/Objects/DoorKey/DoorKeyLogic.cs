using UnityEngine;

public class DoorKeyLogic : MonoBehaviour
{
    [SerializeField] private GameObject visuals;

    public delegate void DoorKeyEvent(DoorKeyLogic key);
    public static event DoorKeyEvent OnKeyCollected;
    public static event DoorKeyEvent OnKeyInitialised;

    private void Start()
    {
        OnKeyInitialised?.Invoke(this);
    }

    public void Collect()
    {
        OnKeyCollected?.Invoke(this);
        Destroy(gameObject);
    }
}
