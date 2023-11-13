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
    [SerializeField] private AnimationClip bulbWallCling;
    [SerializeField] private AnimationClip spiritMove;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private const float MIN_VERT_VELOCITY_FOR_MOVEMENT = 2f;

    private void OnEnable()
    {
        BulbMovement.OnPlayerJump += OnPlayerJump;
        BulbMovement.OnPlayerWallJump += OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround += OnPlayerHitGround;
    }

    private void OnDisable()
    {
        BulbMovement.OnPlayerJump -= OnPlayerJump;
        BulbMovement.OnPlayerWallJump -= OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround -= OnPlayerHitGround;
    }

    const float X_STRETCH_AFTER_JUMP = 0.6f;
    const float Y_STRETCH_AFTER_JUMP = 1.4f;
    private void OnPlayerJump()
    {
        transform.localScale = new Vector3(X_STRETCH_AFTER_JUMP, Y_STRETCH_AFTER_JUMP, transform.localScale.z);
    }

    private void OnPlayerWallJump()
    {
        transform.localScale = new Vector3(X_STRETCH_AFTER_JUMP, Y_STRETCH_AFTER_JUMP, transform.localScale.z);
    }

    const float X_STRETCH_AFTER_FALL = 1.4f;
    const float Y_STRETCH_AFTER_FALL = 0.6f;
    private void OnPlayerHitGround()
    {
        transform.localScale = new Vector3(X_STRETCH_AFTER_FALL, Y_STRETCH_AFTER_FALL, transform.localScale.z);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        NormalizeScale();

        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();

        Vector2 lastDesiredVelocity = controller.GetLastDesiredVelocity();
        bool grounded = controller.IsGrounded();
        bool huggingWall = controller.collisions.left || controller.collisions.right;

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.right = controller.GetLastDesiredVelocity().normalized;
        }
        else if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            FlipBulbWhenAppropriate(lastDesiredVelocity);

            if (Mathf.Abs(lastDesiredVelocity.y) / Time.deltaTime > MIN_VERT_VELOCITY_FOR_MOVEMENT || !grounded)
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
                    if (huggingWall)
                    {
                        animator.Play(bulbWallCling.name);
                        spriteRenderer.flipX = !spriteRenderer.flipX;
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

    [SerializeField] private float scaleResetTime = 1.75f;

    private void NormalizeScale()
    {
        float xScale = transform.localScale.x;
        float yScale = transform.localScale.y;
        float zScale = transform.localScale.z;

        xScale = Mathf.MoveTowards(xScale, 1f, scaleResetTime * Time.unscaledDeltaTime);
        yScale = Mathf.MoveTowards(yScale, 1f, scaleResetTime * Time.unscaledDeltaTime);

        transform.localScale = new Vector3(xScale, yScale, zScale);
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
