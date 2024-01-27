using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorLogic : MonoBehaviour
{
    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;
    [SerializeField] private PlayerDetectorTrigger detector;
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    [SerializeField] private float openDistance;
    [SerializeField] private bool startOpen;
    [SerializeField] private SpriteRenderer counterSprite;

    private bool active;
    private bool vertical;

    private float openProgressPerSecond;
    private float closeProgressPerSecond;

    private float percentOpen;

    private Vector2 door1ClosedPosition;
    private Vector2 door1OpenPosition;
    private Vector2 door2ClosedPosition;
    private Vector2 door2OpenPosition;

    private int keys = 0;

    private int maxKeys = -1;

    private float currentVisualPercentage = 0;
    private float targetVisualPercentage = 1;

    private void OnEnable()
    {
        DoorKeyLogic.OnKeyInitialised += OnKeyInit;
        DoorKeyLogic.OnKeyCollected += OnKeyPickup;
    }

    private void OnDisable()
    {
        DoorKeyLogic.OnKeyInitialised -= OnKeyInit;
        DoorKeyLogic.OnKeyCollected -= OnKeyPickup;
    }

    private void OnKeyInit(DoorKeyLogic key)
    {
        keys++;
    }

    private void OnKeyPickup(DoorKeyLogic key)
    {
        if (maxKeys < 0)
        {
            maxKeys = keys;
        }
        keys--;
        UpdateVisualsTarget();
        if (keys <= 0)
        {
            Activate();
        }
    }

    private void Start()
    {
        active = false;
        vertical = door1.position.y - door2.position.y != 0;
        percentOpen = 0;

        openProgressPerSecond = 1 / openTime;
        closeProgressPerSecond = 1 / closeTime;

        if (startOpen)
        {
            door1OpenPosition = door1.position;
            door2OpenPosition = door2.position;
            door1ClosedPosition = door1OpenPosition + (vertical ? Vector2.up : Vector2.left) * openDistance;
            door2ClosedPosition = door2OpenPosition + (vertical ? Vector2.down : Vector2.right) * openDistance;
        }
        else
        {
            door1ClosedPosition = door1.position;
            door2ClosedPosition = door2.position;
            door1OpenPosition = door1ClosedPosition + (vertical ? Vector2.up : Vector2.left) * openDistance;
            door2OpenPosition = door2ClosedPosition + (vertical ? Vector2.down : Vector2.right) * openDistance;
        }

        UpdateVisualsTarget();
    }

    private void Update()
    {
        CalculateOpenPercent();
        ApplyOpen();
        UpdateVisuals();
    }

    private const float FILL_ANIMATION_SPEED = 1f;

    private void UpdateVisuals()
    {
        currentVisualPercentage = Mathf.MoveTowards(currentVisualPercentage, targetVisualPercentage, FILL_ANIMATION_SPEED * Time.unscaledDeltaTime);
        counterSprite.material.SetFloat("_FillPercent", currentVisualPercentage);
    }

    private void UpdateVisualsTarget()
    {
        float progress = 1 - ((float)keys / maxKeys);
        if (maxKeys == -1)
        {
            progress = 0;
        }
        targetVisualPercentage = progress;
    }

    private void ApplyOpen()
    {
        door1.position = Vector2.Lerp(door1ClosedPosition, door1OpenPosition, percentOpen);
        door2.position = Vector2.Lerp(door2ClosedPosition, door2OpenPosition, percentOpen);
    }

    private void CalculateOpenPercent()
    {
        if (!detector.IsPlayerInside())
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
