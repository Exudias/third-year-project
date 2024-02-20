using UnityEngine;

public class EmptyBulbLogic : MonoBehaviour
{
    [SerializeField] private bool hasCooldown = true;
    [SerializeField] private float cooldownLength = 0.5f;
    [SerializeField] private Controller2D controller;
    [SerializeField] private GameObject destroyParticles;
    [SerializeField] private Transform visualsTransform;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private SoundEmitterLogic soundEmitter;

    private float currentCooldown;

    private float gravity = 0;
    private float terminalVelocity = 0;
    private float deceleration = 0;
    private Vector2 velocity = Vector2.zero;
    private bool destroyOnSolid = false;

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
        if (destroyOnSolid && dir == Vector2.down)
        {
            DestroySelf();
        }
    }

    private void Start()
    {
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

    public void SetDestroyOnSolid(bool newSetting)
    {
        destroyOnSolid = newSetting;
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
        Destroy(transform.parent.gameObject);
    }

    public void DestroySelf()
    {
        // Visuals
        GameObject deathObj = Instantiate(destroyParticles, transform.position, Quaternion.identity);
        GameManager.MoveObjectToLevelScene(deathObj);
        // Sound
        SoundEmitterLogic emitterObj = Instantiate(soundEmitter, transform.position, Quaternion.identity);
        emitterObj.PlaySound(breakSound, 1f);
        GameManager.MoveObjectToLevelScene(emitterObj.gameObject);
        // Destroy actual bulb
        Destroy(transform.parent.gameObject);
    }

    public void SetSpriteOffset(Vector2 offset)
    {
        visualsTransform.localPosition = offset;
    }
}
