using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BulbSpawnerVisuals : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite emptySprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MakeEmpty()
    {
        spriteRenderer.sprite = emptySprite;
    }
}
