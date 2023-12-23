using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerVisualsManager : MonoBehaviour
{
    [SerializeField] private PlayerFormSwitcher formSwitcher;
    [SerializeField] private Controller2D controller;
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private PlayerSound sound;

    [SerializeField] private Vector2 bulbSpriteOffset = new Vector3(0f, -0.5f, 0f);
    [SerializeField] private Vector2 spiritSpriteOffset = new Vector3(0f, 0f, 0f);

    [SerializeField] private Material bulbMaterial;
    [SerializeField] private Material spiritMaterial;

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
    private TrailRenderer trailRenderer;

    private const float TRAIL_MAX_WIDTH = 0.5f;

    public delegate void StepEvent();
    public static event StepEvent OnPlayerFootstep;

    private void OnEnable()
    {
        BulbMovement.OnPlayerJump += OnPlayerJump;
        BulbMovement.OnPlayerWallJump += OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround += OnPlayerHitGround;
        PlayerFormSwitcher.OnSwitchToSpirit += OnSwitchToSpirit;
    }

    private void OnDisable()
    {
        BulbMovement.OnPlayerJump -= OnPlayerJump;
        BulbMovement.OnPlayerWallJump -= OnPlayerWallJump;
        BulbMovement.OnPlayerHitGround -= OnPlayerHitGround;
        PlayerFormSwitcher.OnSwitchToSpirit -= OnSwitchToSpirit;
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

    const float SCALE_AFTER_SPIRIT = 0.2f;
    private void OnSwitchToSpirit()
    {
        transform.localScale = new Vector3(SCALE_AFTER_SPIRIT, SCALE_AFTER_SPIRIT, transform.localScale.z);
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
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void UpdateSpriteOffset()
    {
        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.localPosition = spiritSpriteOffset;
            transform.right = controller.GetLastDesiredVelocity().normalized;
        }
        else if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            transform.localPosition = bulbSpriteOffset;
        }
    }

    private const float TRAIL_MIN_WIDTH = 0.1f;

    private void Update()
    {
        if (GameManager.IsGamePaused()) return;

        NormalizeScale();

        PlayerFormSwitcher.PlayerForm form = formSwitcher.GetCurrentForm();

        Vector2 lastDesiredVelocity = controller.GetLastDesiredVelocity();
        bool grounded = controller.IsGrounded();
        bool huggingWall = controller.collisions.left || controller.collisions.right;

        if (energyManager != null && energyManager.enabled)
        {
            spriteRenderer.material.SetFloat("_PercentEnergy", energyManager.GetEnergyPercent());
        }
        else
        {
            spriteRenderer.material.SetFloat("_PercentEnergy", 1);
        }

        if (form == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            transform.right = controller.GetLastDesiredVelocity().normalized;
            trailRenderer.widthMultiplier = Mathf.Clamp(TRAIL_MAX_WIDTH * energyManager.GetEnergyPercent(), TRAIL_MIN_WIDTH, 1);
            trailRenderer.time = Time.timeScale;
        }
        else if (form == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            FlipBulbWhenAppropriate(lastDesiredVelocity);

            if (!grounded)
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
                        FlipXScaleSign();
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

        if (xScale > 0)
        {
            xScale = Mathf.MoveTowards(xScale, 1f, scaleResetTime * Time.unscaledDeltaTime);
        }
        else if (xScale < 0)
        {
            xScale = Mathf.MoveTowards(xScale, -1f, scaleResetTime * Time.unscaledDeltaTime);
        }

        
        yScale = Mathf.MoveTowards(yScale, 1f, scaleResetTime * Time.unscaledDeltaTime);

        transform.localScale = new Vector3(xScale, yScale, zScale);
    }

    private void FlipBulbWhenAppropriate(Vector2 lastDesiredVelocity)
    {
        bool xScaleIsNegative = transform.localScale.x < 0;
        if (xScaleIsNegative)
        {
            if (lastDesiredVelocity.x > 0)
            {
                MakeXScalePositive();
            }
        }
        else
        {
            if (lastDesiredVelocity.x < 0)
            {
                MakeXScaleNegative();
            }
        }
    }

    public void InitBulb()
    {
        spriteRenderer.material = bulbMaterial;
        transform.rotation = Quaternion.identity;
        animator.Play(bulbIdleForward.name);
        trailRenderer.emitting = false;
        UpdateSpriteOffset();
    }

    public void InitSpirit()
    {
        spriteRenderer.material = spiritMaterial;
        MakeXScalePositive();
        animator.Play(spiritMove.name);
        trailRenderer.emitting = true;
        UpdateSpriteOffset();
    }

    private void FlipXScaleSign()
    {
        float xScale = transform.localScale.x;
        xScale *= -1;
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }

    private void MakeXScaleNegative()
    {
        float xScale = transform.localScale.x;
        xScale = -Mathf.Abs(xScale);
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }

    private void MakeXScalePositive()
    {
        float xScale = transform.localScale.x;
        xScale = Mathf.Abs(xScale);
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }

    public void RegisterStep()
    {
        OnPlayerFootstep?.Invoke();
    }
}
