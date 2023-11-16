using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonVisualsManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private Sprite pressedSprite;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnButtonPressed()
    {
        spriteRenderer.sprite = pressedSprite;
    }

    public void OnButtonUnpressed()
    {
        spriteRenderer.sprite = unpressedSprite;
    }
}
