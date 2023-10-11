using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerVisualsManager : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [SerializeField] private Controller2D controller;

    [SerializeField] private Sprite triangle;
    [SerializeField] private Sprite square;

    [SerializeField] private AnimationClip bulbIdleForward;
    [SerializeField] private AnimationClip spiritMove;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        transform.rotation = Quaternion.identity;
        animator.Play(bulbIdleForward.name);
    }

    public void InitSpirit()
    {
        spriteRenderer.sprite = triangle;
        animator.Play(spiritMove.name);
    }
}
