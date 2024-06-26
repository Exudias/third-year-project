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
    [SerializeField] private float terminalWallVelocity = 5f;
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

    private Vector2 spiritLastDirection;
    private float externalHorizontalVelocity;

    private Controller2D controller;
    private EnergyManager energyManager;
    private InputManager input;
    private SpiritMovement spiritMovement;

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
        if (spiritMovement == null)
        {
            spiritMovement = GetComponent<SpiritMovement>();
        }

        spiritLastDirection = spiritMovement.GetMovementDir();

        coyoteTime = Mathf.Infinity;
        timeSinceWallJump = Mathf.Infinity;
        timeSinceSuperJumpTransformation = Mathf.Infinity;

        velocity = Vector2.zero;
        externalHorizontalVelocity = 0;

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

    private void OnControllerCollision(Controller2D source, Vector2 dir)
    {
        if (source != controller) return;
        // If horizontal collision, stop counting as walljump
        if (dir.x != 0)
        {
            timeSinceWallJump = Mathf.Infinity;
        }
    }

    private void Update()
    {
        if (GameManager.IsGamePaused() || GameManager.IsPlayerDead() || GameManager.IsLoadingScene()) return;

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
            if (Time.deltaTime != 0 && Mathf.Abs(lastYVelocity) > minVelocityToHitGround && !GameManager.IsLoadingScene())
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
        float timeSinceJumpPress = input.SecondsSincePressed(KeyCode.Space);
        // time since jump press is <0 if hasn't ever been pressed
        if (timeSinceJumpPress <= jumpBufferLeniency && timeSinceJumpPress >= 0)
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

        float xMax = targetVelocityX + externalHorizontalVelocity;
        bool hasExternalForce = externalHorizontalVelocity != 0;
        bool externalWithUs = targetVelocityX * externalHorizontalVelocity > 0;
        if (hasExternalForce && externalWithUs)
        {
            accelToUse *= Mathf.Abs(xMax / targetVelocityX);
        }
        bool fasterThanCap = Mathf.Abs(velocity.x) > Mathf.Abs(targetVelocityX);
        bool movingWithCap = velocity.x * targetVelocityX > 0;
        if (!hasExternalForce && fasterThanCap)
        {
            if (movingWithCap)
            {
                accelToUse *= .5f;
            }
            else
            {
                float t = timeSinceWallJump / 1.5f;
                float accelMult = Mathf.Lerp(1, 5, t);
                accelToUse *= accelMult;
            }
        }

        velocity.x = Mathf.MoveTowards(velocity.x, xMax, accelToUse * Time.deltaTime);

        bool huggingWall = controller.collisions.left || controller.collisions.right;

        float wallGravityMultiplier = (huggingWall && velocity.y < 0) ? huggingWallGravityMultiplier : 1f;

        float gravityStep = gravity * wallGravityMultiplier * Time.deltaTime;

        float terminalVelocityToUse = huggingWall ? -terminalWallVelocity : -terminalVelocity;

        velocity.y = Mathf.Clamp(velocity.y + gravityStep, terminalVelocityToUse, Mathf.Infinity);

        controller.Move(velocity * Time.deltaTime);
    }

    public void SetExternalVelocity(float newValue)
    {
        externalHorizontalVelocity = newValue;
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
            // Jump in direction of input at time of jump, or if no input - direction of last spirit's velocity
            float superDirection = input.GetHorizontalRaw() == 0 ? Mathf.Sign(spiritLastDirection.x) : Mathf.Sign(input.GetHorizontalRaw());
            velocity.x = superDirection * horizontalSuperJumpBoost;
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

    public float GetGravity() => gravity;

    public float GetTerminalVelocity() => terminalVelocity;

    public float GetDeceleration() => deceleration;
}
