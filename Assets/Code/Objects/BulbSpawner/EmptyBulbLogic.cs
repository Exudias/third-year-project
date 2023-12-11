using UnityEngine;

public class EmptyBulbLogic : MonoBehaviour
{
    [SerializeField] private bool hasCooldown = true;
    [SerializeField] private float cooldownLength = 0.5f;

    private Controller2D controller;

    private float currentCooldown;

    private Vector2 velocity = Vector2.zero;

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
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
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
