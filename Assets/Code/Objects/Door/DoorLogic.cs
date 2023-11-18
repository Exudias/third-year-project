using UnityEngine;

public class DoorLogic : MonoBehaviour, IActivatable
{
    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    [SerializeField] private float openDistance;

    private bool active;
    private bool vertical;

    private float openProgressPerSecond;
    private float closeProgressPerSecond;

    private float percentOpen;

    private Vector2 door1ClosedPosition;
    private Vector2 door1OpenPosition;
    private Vector2 door2ClosedPosition;
    private Vector2 door2OpenPosition;

    private void Start()
    {
        active = false;
        vertical = door1.position.y - door2.position.y != 0;
        percentOpen = 0;

        openProgressPerSecond = 1 / openTime;
        closeProgressPerSecond = 1 / closeTime;

        door1ClosedPosition = door1.position;
        door2ClosedPosition = door2.position;
        door1OpenPosition = door1ClosedPosition + (vertical ? Vector2.up : Vector2.left) * openDistance; 
        door2OpenPosition = door2ClosedPosition + (vertical ? Vector2.down : Vector2.right) * openDistance; 
    }

    private void Update()
    {
        CalculateOpenPercent();
        ApplyOpen();
    }

    private void ApplyOpen()
    {
        door1.position = Vector2.Lerp(door1ClosedPosition, door1OpenPosition, percentOpen);
        door2.position = Vector2.Lerp(door2ClosedPosition, door2OpenPosition, percentOpen);
    }

    private void CalculateOpenPercent()
    {
        if (active)
        {
            percentOpen += openProgressPerSecond * Time.deltaTime;
        }
        else
        {
            percentOpen -= closeProgressPerSecond * Time.deltaTime;
        }
        percentOpen = Mathf.Clamp01(percentOpen);
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }
}
