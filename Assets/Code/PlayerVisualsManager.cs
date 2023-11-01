using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerVisualsManager : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [SerializeField] private Controller2D controller;

    [SerializeField] private AnimationClip bulbIdleForward;
    [SerializeField] private AnimationClip bulbWalk;
    [SerializeField] private AnimationClip bulbJumpForward;
    [SerializeField] private AnimationClip bulbFallForward;
    [SerializeField] private AnimationClip bulbJumpSide;
    [SerializeField] private AnimationClip bulbFallSide;
    [SerializeField] private AnimationClip spiritMove;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private const float MIN_VERT_VELOCITY_FOR_MOVEMENT = 0.01f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();

        Vector2 lastDesiredVelocity = controller.GetLastDesiredVelocity();
        bool grounded = controller.IsGrounded();

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.right = controller.GetLastDesiredVelocity().normalized;
        }
        else if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            FlipBulbWhenAppropriate(lastDesiredVelocity);

            if (Mathf.Abs(lastDesiredVelocity.y) > MIN_VERT_VELOCITY_FOR_MOVEMENT || !grounded)
            {
                if (lastDesiredVelocity.y > 0)
                {
                    if (lastDesiredVelocity.x != 0)
                    {
                        animator.Play(bulbJumpSide.name);
                    }
                    else
                    {
                        animator.Play(bulbJumpForward.name);
                    }    
                }
                else
                {
                    if (lastDesiredVelocity.x != 0)
                    {
                        animator.Play(bulbFallSide.name);
                    }
                    else
                    {
                        animator.Play(bulbFallForward.name);
                    }
                }
            }
            else
            {
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
    }

    private void FlipBulbWhenAppropriate(Vector2 lastDesiredVelocity)
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
