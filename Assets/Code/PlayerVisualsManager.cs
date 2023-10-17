using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerVisualsManager : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [SerializeField] private Controller2D controller;

    [SerializeField] private AnimationClip bulbIdleForward;
    [SerializeField] private AnimationClip bulbWalk;
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

        Vector2 lastDesiredVelocity = controller.GetLastDesiredVelocity();

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.right = controller.GetLastDesiredVelocity().normalized;
        }
        else if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            if (spriteRenderer.flipX)
            {
                if (lastDesiredVelocity.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else
            {
                if (lastDesiredVelocity.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }

            if (lastDesiredVelocity.x != 0)
            {
                animator.Play(bulbWalk.name);
            }
            else
            {
                animator.Play(bulbIdleForward.name);
            }
        }
    }

    public void InitBulb()
    {
        transform.rotation = Quaternion.identity;
        animator.Play(bulbIdleForward.name);
    }

    public void InitSpirit()
    {
        spriteRenderer.flipX = false;
        animator.Play(spiritMove.name);
    }
}
