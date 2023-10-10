using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualsManager : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [SerializeField] private Controller2D controller;

    [SerializeField] private Sprite triangle;
    [SerializeField] private Sprite square;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.right = controller.GetLastDesiredVelocity().normalized;
        }
    }

    public void InitBulb()
    {
        spriteRenderer.color = new Color(0.6f, 0.15f, 0.15f);
        spriteRenderer.sprite = square;
        transform.rotation = Quaternion.identity;
    }

    public void InitSpirit()
    {
        spriteRenderer.color = new Color(0.7f, 0.7f, 0);
        spriteRenderer.sprite = triangle;
    }
}
