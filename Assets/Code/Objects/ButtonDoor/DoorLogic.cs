using UnityEngine;
using System.Collections.Generic;

public class DoorLogic : MonoBehaviour, IActivatable
{
    [Header("References")]
    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;
    [SerializeField] private PlayerDetectorTrigger detector;
    [Header("Parameters")]
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    [SerializeField] private float openDistance;
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer counterSprite;
    [SerializeField] private GameObject cableHolder;
    [SerializeField] private Color cableOffColor;
    [SerializeField] private Color cableOnColor;

    private bool active;
    private bool vertical;

    private float openProgressPerSecond;
    private float closeProgressPerSecond;

    private float percentOpen;

    private Vector2 door1ClosedPosition;
    private Vector2 door1OpenPosition;
    private Vector2 door2ClosedPosition;
    private Vector2 door2OpenPosition;

    private List<SpriteRenderer> cableFGSprites;

    private void Start()
    {
        active = false;
        vertical = Mathf.Abs(door1.position.y - door2.position.y) > Mathf.Abs(door1.position.x - door2.position.x);
        percentOpen = 0;

        openProgressPerSecond = 1 / openTime;
        closeProgressPerSecond = 1 / closeTime;

        door1ClosedPosition = door1.position;
        door2ClosedPosition = door2.position;
        door1OpenPosition = door1ClosedPosition + (vertical ? Vector2.up : Vector2.left) * openDistance; 
        door2OpenPosition = door2ClosedPosition + (vertical ? Vector2.down : Vector2.right) * openDistance;

        cableFGSprites = GetFGCables();
    }

    private List<SpriteRenderer> GetFGCables()
    {
        List<SpriteRenderer> fgCables = new List<SpriteRenderer>();

        // If no cables, return empty list
        if (cableHolder == null) return fgCables;

        int cableCount = cableHolder.transform.childCount;
        for (int i = 0; i < cableCount; i++)
        {
            Transform cable = cableHolder.transform.GetChild(i);
            // Get child of cable, which is the actual sprite of the cable's "energy"
            fgCables.Add(cable.GetChild(0).GetComponent<SpriteRenderer>());
        }

        return fgCables;
    }

    private void Update()
    {
        CalculateOpenPercent();
        ApplyOpen();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Circle counter
        counterSprite.material.SetFloat("_FillPercent", percentOpen);
        // Button cables
        Color cableColor = Color.Lerp(cableOffColor, cableOnColor, percentOpen);
        for (int i = 0; i < cableFGSprites.Count; i++)
        {
            SpriteRenderer cable = cableFGSprites[i];
            cable.color = cableColor;
        }
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
