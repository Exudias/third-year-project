using UnityEngine;

// Controller2D class based on Sebastian Lague's (https://www.youtube.com/@SebastianLague) 2D platformer series
// The original repo can be found here: https://github.com/SebLague/2DPlatformer-Tutorial
// Sebastian has provided the code with an MIT license

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public delegate void CollisionEvent(Vector2 direction);
    public delegate void DeathCollisionEvent(Vector2 direction, bool isDirectionalDeath, GameObject killer);
    public static event CollisionEvent OnCollision;
    public static event DeathCollisionEvent OnDeathCollision;

    const float SKIN_WIDTH = 0.015f;
    const int MIN_RAYS = 2;
    const int MAX_RAYS = 10;

    private bool DEBUG = true;

    [Header("Collision Parameters")]
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int horizontalRayCount = 4;
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask deathMask;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    private BoxCollider2D coll;
    private RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    private Vector2 lastDesiredVelocity;
    private Vector2 lastActualVelocity;

    private void Start()
    {
        UpdateCollider();
        lastDesiredVelocity = Vector2.zero;
        lastActualVelocity = Vector2.zero;
    }

    public void UpdateCollider()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders)
        {
            if (collider.enabled)
            {
                coll = collider;
                break;
            }
        }
        CalculateRaySpacing();
    }

    public Vector2 GetLastDesiredVelocity()
    {
        return lastDesiredVelocity;
    }

    public Vector2 GetLastActualVelocity()
    {
        return lastActualVelocity;
    }

    public void Move(Vector2 velocity, GameObject pusher = null)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        lastDesiredVelocity = velocity;

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        lastActualVelocity = velocity;

        transform.Translate(velocity);
    }

    public bool CanWallJump(float leniency, int direction)
    {
        float rayLength = leniency + SKIN_WIDTH;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (direction == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, collisionMask);

            if (hit) return true;
        }
        return false;
    }

    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;

        Collider2D deathCollider = null;
        float closestHit = Mathf.Infinity;
        float closestDeathHit = Mathf.Infinity;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            RaycastHit2D deathHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, deathMask);

            if (DEBUG)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            }

            if (hit)
            {
                if (closestHit > hit.distance)
                {
                    closestHit = hit.distance;
                }

                velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
                rayLength = hit.distance;

                if (directionX == -1)
                {
                    OnCollision?.Invoke(Vector2.left);
                    collisions.left = true;
                    collisions.right = false;
                }
                else if (directionX == 1)
                {
                    OnCollision?.Invoke(Vector2.right);
                    collisions.left = false;
                    collisions.right = true;
                }
            }

            if (deathHit)
            {
                if (closestDeathHit > deathHit.distance)
                {
                    closestDeathHit = deathHit.distance;
                    deathCollider = deathHit.collider;
                }
            }
        }

        bool hitDeath = closestDeathHit < closestHit;

        if (hitDeath)
        {
            bool isDirectionalDeath = deathCollider.gameObject.GetComponent<DirectionalKiller>() != null;
            OnDeathCollision?.Invoke(lastDesiredVelocity.normalized, isDirectionalDeath, deathCollider.gameObject);
        }
    }

    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

        Collider2D deathCollider = null;
        float closestHit = Mathf.Infinity;
        float closestDeathHit = Mathf.Infinity;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            RaycastHit2D deathHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, deathMask);

            if (DEBUG)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            }

            if (hit)
            {
                if (closestHit > hit.distance)
                {
                    closestHit = hit.distance;
                }

                velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
                rayLength = hit.distance;

                if (directionY == -1)
                {
                    OnCollision?.Invoke(Vector2.down);
                    collisions.bottom = true;
                    collisions.top = false;
                }
                else if (directionY == 1)
                {
                    OnCollision?.Invoke(Vector2.up);
                    collisions.bottom = false;
                    collisions.top = true;
                }
            }

            if (deathHit)
            {
                if (closestDeathHit > deathHit.distance)
                {
                    closestDeathHit = deathHit.distance;
                    deathCollider = deathHit.collider;
                }
            }
        }

        bool hitDeath = closestDeathHit < closestHit;

        if (hitDeath)
        {
            bool isDirectionalDeath = deathCollider.gameObject.GetComponent<DirectionalKiller>() != null;
            OnDeathCollision?.Invoke(lastDesiredVelocity.normalized, isDirectionalDeath, deathCollider.gameObject);
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = GetInsetBounds();

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = GetInsetBounds();

        // Clamp ray count if somehow OOB
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, MIN_RAYS, MAX_RAYS);
        verticalRayCount = Mathf.Clamp(verticalRayCount, MIN_RAYS, MAX_RAYS);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private Bounds GetInsetBounds()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(SKIN_WIDTH * -2);
        return bounds;
    }

    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool top, bottom;
        public bool left, right;

        public void Reset()
        {
            top = bottom = false;
            left = right = false;
        }
    }
}
