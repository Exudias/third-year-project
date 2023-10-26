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
    private float timeSinceJumpPressed;
    private bool jumping;
    private float timeSinceWallJump;
    private float timeSinceSuperJumpTransformation;

    private Controller2D controller;
    private EnergyManager energyManager;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        energyManager = GetComponent<EnergyManager>();

        CalculateGravityAndJumpVelocity();
        timeSinceJumpPressed = Mathf.Infinity;
        coyoteTime = Mathf.Infinity;
        timeSinceWallJump = Mathf.Infinity;
        timeSinceSuperJumpTransformation = Mathf.Infinity;
    }

    // Used when switching from spirit to reset to initial state
    private void ResetForFreshBulb()
    {
        timeSinceJumpPressed = Mathf.Infinity;
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
        timeSinceJumpPressed += Time.deltaTime;
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
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSinceJumpPressed = 0;
        }

        if (jumping && !Input.GetKey(KeyCode.Space))
        {
            EndJump();
        }

        // regular jump
        if (coyoteTime <= coyoteJumpLeniency && timeSinceJumpPressed <= jumpBufferLeniency)
        {
            Jump();
        }

        const int DIR_LEFT = -1;
        const int DIR_RIGHT = 1;

        //wall jump
        if (timeSinceJumpPressed <= jumpBufferLeniency)
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
        velocity.y = maxJumpVelocity;
        timeSinceJumpPressed = Mathf.Infinity;
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
        velocity.x = wallJumpStrength * direction;
        velocity.y = maxJumpVelocity * wallJumpHeightMultiplier;
        timeSinceJumpPressed = Mathf.Infinity;
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
