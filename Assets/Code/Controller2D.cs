using UnityEngine;

// Controller2D class based on Sebastian Lague's (https://www.youtube.com/@SebastianLague) 2D platformer series
// The original repo can be found here: https://github.com/SebLague/2DPlatformer-Tutorial
// Sebastian has provided the code with an MIT license

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    const float SKIN_WIDTH = 0.015f;
    const int MIN_RAYS = 2;
    const int MAX_RAYS = 10;

    private bool DEBUG = false;

    [Header("Collision Parameters")]
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int horizontalRayCount = 4;
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    private BoxCollider2D coll;
    private RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        CalculateRaySpacing(); // only need to call if # rays is changed
    }

    public void Move(Vector2 velocity, GameObject pusher = null)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

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

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (DEBUG)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            }

            if (hit)
            {
                velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (DEBUG)
            {
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            }

            if (hit)
            {
                velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
                rayLength = hit.distance;

                collisions.bottom = directionY == -1;
                collisions.top = directionY == 1;
            }
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
