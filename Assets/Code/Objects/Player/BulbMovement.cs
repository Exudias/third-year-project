using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(EnergyManager))]
public class BulbMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float minTopSpeed = 6f;
    [SerializeField] private float maxTopSpeed = 12f;
    [SerializeField] private float acceleration = 100;
    [SerializeField] private float deceleration = 160;
    [SerializeField] private float airControlMult = 0.7f;
    [Header("Vertical Movement")]
    [SerializeField] private float maxJumpHeight = 3;
    [SerializeField] private float minJumpHeight = 0.5f;
    [SerializeField] private float timeToJumpApex = 0.3f;
    [SerializeField] private float jumpBufferLeniency = 0.1f;
    [SerializeField] private float coyoteJumpLeniency = 0.1f;
    [SerializeField] private float terminalVelocity = 10f;
    [SerializeField] private float minVelocityToHitGround = 1f;
    [Header("Wall Jump")]
    [SerializeField] private float wallJumpLeniency = 0.125f;
    [SerializeField] private float wallJumpStrength = 12f;
    [SerializeField] private float wallJumpHeightMultiplier = 0.8f;
    [SerializeField] private float wallJumpNoControlTime = 0.2f;
    [SerializeField] private float huggingWallGravityMultiplier = 0.4f;
    [Header("Transform Movement")]
    [SerializeField] private float superJumpMaxTime = 0.1f;
    [SerializeField] private float horizontalSuperJumpBoost = 20f;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector2 velocity;
    private float coyoteTime;
    private bool jumping;
    private float timeSinceWallJump;
    private float timeSinceSuperJumpTransformation;

    private Controller2D controller;
    private EnergyManager energyManager;
    private InputManager input;

    public delegate void JumpEvent();
    public delegate void FallEvent();
    public static event JumpEvent OnPlayerJump;
    public static event JumpEvent OnPlayerWallJump;
    public static event FallEvent OnPlayerHitGround;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        energyManager = GetComponent<EnergyManager>();
        input = InputManager.instance;

        CalculateGravityAndJumpVelocity();
        coyoteTime = Mathf.Infinity;
        timeSinceWallJump = Mathf.Infinity;
        timeSinceSuperJumpTransformation = Mathf.Infinity;
    }

    // Used when switching from spirit to reset to initial state
    private void ResetForFreshBulb()
    {
        coyoteTime = Mathf.Infinity;
        timeSinceWallJump = Mathf.Infinity;
        timeSinceSuperJumpTransformation = Mathf.Infinity;

        velocity = Vector2.zero;

        if (controller != null)
        {
            Vector2 lastVelocity = controller.GetLastActualVelocity();
            if (lastVelocity.x != 0)
            {
                timeSinceSuperJumpTransformation = 0;
            }
        }
    }

    private void OnEnable()
    {
        Controller2D.OnCollision += OnControllerCollision;

        ResetForFreshBulb();
    }

    private void OnDisable()
    {
        Controller2D.OnCollision -= OnControllerCollision;
    }

    private void OnControllerCollision(Vector2 dir)
    {
        // If horizontal collision, stop counting as walljump
        if (dir.x != 0)
        {
            timeSinceWallJump = Mathf.Infinity;
        }
    }

    private void Update()
    {
        coyoteTime += Time.deltaTime;
        timeSinceWallJump += Time.deltaTime;
        timeSinceSuperJumpTransformation += Time.deltaTime;

        float topSpeed = Mathf.Lerp(minTopSpeed, maxTopSpeed, energyManager.GetEnergyPercent());

        if (controller.collisions.top || controller.collisions.bottom)
        {
            velocity.y = 0;
        }

        if (controller.collisions.bottom)
        {
            coyoteTime = 0;
            jumping = false;

            float lastYVelocity = controller.GetLastActualVelocity().y / Time.deltaTime;
            if (Mathf.Abs(lastYVelocity) > minVelocityToHitGround)
            {
                OnPlayerHitGround?.Invoke();
            }
        }

        float horizontalInput = input.GetHorizontalRaw();

        if (jumping && !input.IsPressed(KeyCode.Space))
        {
            EndJump();
        }

        // regular jump
        if (coyoteTime <= coyoteJumpLeniency && input.SecondsSincePressed(KeyCode.Space) <= jumpBufferLeniency)
        {
            Jump();
        }

        const int DIR_LEFT = -1;
        const int DIR_RIGHT = 1;

        //wall jump
        if (input.SecondsSincePressed(KeyCode.Space) <= jumpBufferLeniency)
        {
            if (controller.CanWallJump(wallJumpLeniency, DIR_LEFT))
            {
                WallJump(DIR_RIGHT);
            }
            else if (controller.CanWallJump(wallJumpLeniency, DIR_RIGHT))
            {
                WallJump(DIR_LEFT);
            }
        }

        float targetVelocityX = horizontalInput * topSpeed;
        float accelToUse = (targetVelocityX * horizontalInput <= 0) ? deceleration : acceleration;
        accelToUse *= controller.collisions.bottom ? 1 : airControlMult;

        // if just walljumped, take away control/friction
        if (timeSinceWallJump < wallJumpNoControlTime)
        {
            accelToUse = 0;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, accelToUse * Time.deltaTime);

        bool huggingWall = controller.collisions.left || controller.collisions.right;

        float wallGravityMultiplier = (huggingWall && velocity.y < 0) ? huggingWallGravityMultiplier : 1f;

        float gravityStep = gravity * wallGravityMultiplier * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y + gravityStep, -terminalVelocity, Mathf.Infinity);

        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        OnPlayerJump?.Invoke();
        input.ConsumeBuffer(KeyCode.Space);
        velocity.y = maxJumpVelocity;
        coyoteTime = Mathf.Infinity;
        jumping = true;

        bool jumpShouldBeSuper = timeSinceSuperJumpTransformation <= superJumpMaxTime;

        if (jumpShouldBeSuper)
        {
            velocity.x = Mathf.Sign(velocity.x) * horizontalSuperJumpBoost;
        }
    }

    private void WallJump(int direction)
    {
        OnPlayerWallJump?.Invoke();
        input.ConsumeBuffer(KeyCode.Space);
        velocity.x = wallJumpStrength * direction;
        velocity.y = maxJumpVelocity * wallJumpHeightMultiplier;
        coyoteTime = Mathf.Infinity;
        timeSinceWallJump = 0;
    }

    private void EndJump()
    {
        velocity.y = Mathf.Min(minJumpVelocity, velocity.y);
    }

    private void CalculateGravityAndJumpVelocity()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
}
