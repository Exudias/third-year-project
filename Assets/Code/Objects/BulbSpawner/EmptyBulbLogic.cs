using UnityEngine;

public class EmptyBulbLogic : MonoBehaviour
{
    [SerializeField] private bool hasCooldown = true;
    [SerializeField] private float cooldownLength = 0.5f;

    private Controller2D controller;

    private float currentCooldown;

    private float gravity = 0;
    private float terminalVelocity = 0;
    private float deceleration = 0;
    private Vector2 velocity = Vector2.zero;

    private void OnEnable()
    {
        Controller2D.OnCollision += OnControllerCollide;
    }

    private void OnDisable()
    {
        Controller2D.OnCollision -= OnControllerCollide;
    }

    private void OnControllerCollide(Controller2D source, Vector2 dir)
    {
        if (source != controller) return;
        Debug.Log("Yeah.");
    }

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        if (hasCooldown)
        {
            currentCooldown = cooldownLength;
        }
    }

    private void Update()
    {
        if (currentCooldown >= 0)
        {
            currentCooldown -= Time.unscaledDeltaTime;
        }
        if (!controller.collisions.bottom)
        {
            float gravityStep = gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y + gravityStep, -terminalVelocity, Mathf.Infinity);
        }
        const float AIR_CONTROL_MULT = 0.2f;

        velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * (controller.collisions.bottom ? 1 : AIR_CONTROL_MULT) * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }

    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }

    public void SetTerminalVelocity(float newTerminalVelocity)
    {
        terminalVelocity = newTerminalVelocity;
    }

    public void SetDeceleration(float newDeceleration)
    {
        deceleration = newDeceleration;
    }

    public bool HasCooldown()
    {
        return currentCooldown > 0;
    }

    public void Collect()
    {
        Destroy(gameObject);
    }
}
