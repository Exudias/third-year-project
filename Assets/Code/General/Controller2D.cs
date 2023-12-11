using UnityEngine;
using System.Collections.Generic;

// Controller2D class based on Sebastian Lague's (https://www.youtube.com/@SebastianLague) 2D platformer series
// The original repo can be found here: https://github.com/SebLague/2DPlatformer-Tutorial
// Sebastian has provided the code with an MIT license

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public delegate void CollisionEvent(Controller2D source, Vector2 direction);
    public delegate void DeathCollisionEvent(Controller2D source, Vector2 direction, bool isDirectionalDeath, GameObject killer);
    public delegate void OtherCollision(Controller2D source, Vector2 direction, GameObject other);
    public delegate void ControllerEvent();
    public static event CollisionEvent OnCollision;
    public static event DeathCollisionEvent OnDeathCollision;
    public static event OtherCollision OnOtherCollision;
    public static event ControllerEvent OnPostControllerMove;

    const float SKIN_WIDTH = 0.015f;
    const float STATIC_SKIN_WIDTH = 0.02f;
    const int MIN_RAYS = 2;
    const int MAX_RAYS = 10;

    const float NOT_MOVING_THRESHOLD = 0.00001f;

    private bool DEBUG = true;

    [Header("Collision Parameters")]
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int horizontalRayCount = 4;
    [SerializeField, Range(MIN_RAYS, MAX_RAYS)] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask deathMask;
    [SerializeField] private LayerMask otherMask;
    [SerializeField] private float collisionLeniencyStep = 1f / 16f;
    [SerializeField] private int collisionLeniencyIterations = 2;

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

    public Collider2D GetCurrentCollider() => coll;

    public Vector2 GetLastDesiredVelocity()
    {
        return lastDesiredVelocity;
    }

    public Vector2 GetLastActualVelocity()
    {
        return lastActualVelocity;
    }

    public bool IsGrounded()
    {
        return collisions.bottom;
    }

    public void UpdatePosition()
    {
        UpdateRaycastOrigins();
    }

    public void MoveImmediate(Vector2 position)
    {
        UpdateRaycastOrigins();
        stopMovementThisFrame = true;
        collisions.Reset();
        transform.position = position;
    }

    bool stopMovementThisFrame = false;

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

        bool notMoving = Mathf.Abs(velocity.x) < NOT_MOVING_THRESHOLD && Mathf.Abs(velocity.y) < NOT_MOVING_THRESHOLD;
        if (notMoving)
        {
            StaticCheck();
        }

        lastActualVelocity = velocity;

        if (stopMovementThisFrame)
        {
            stopMovementThisFrame = false;
            Move(velocity);
            return;
        }

        transform.Translate(velocity);

        OnPostControllerMove?.Invoke();
    }

    private void StaticCheck()
    {
        Bounds bounds = GetStaticCheckBounds();

        Collider2D[] deathColls = Physics2D.OverlapAreaAll(bounds.min, bounds.max, deathMask);
        Collider2D[] otherColls = Physics2D.OverlapAreaAll(bounds.min, bounds.max, otherMask);

        foreach(Collider2D deathColl in deathColls)
        {
            bool isDirectionalDeath = deathColl.GetComponent<DirectionalKiller>() != null;
            OnDeathCollision?.Invoke(this, Vector2.zero, isDirectionalDeath, deathColl.gameObject);
        }

        foreach (Collider2D otherColl in otherColls)
        {
            OnOtherCollision?.Invoke(this, Vector2.zero, otherColl.gameObject);
        }
    }

    public bool CanWallJump(float leniency, int direction)
    {
        UpdateRaycastOrigins();

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

    const float MIN_DISTANCE_TO_DIE = 0.0001f; // if death collision is more than that closer than normal, die

    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + SKIN_WIDTH;

        Collider2D deathCollider = null;
        float closestHit = Mathf.Infinity;
        float closestDeathHit = Mathf.Infinity;
        List<RaycastHit2D> allOtherHits = new List<RaycastHit2D>();

        List<bool> rayHits = new List<bool>();

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            RaycastHit2D deathHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, deathMask);
            allOtherHits.AddRange(Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, otherMask));

            if (DEBUG)
            {
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            }

            float newLength = rayLength + (i == 0 ? 0 : collisionLeniencyStep);
            rayHits.Add(Physics2D.Raycast(rayOrigin, Vector2.right * directionX, newLength, collisionMask));

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
                    OnCollision?.Invoke(this, Vector2.left);
                    collisions.left = true;
                    collisions.right = false;
                }
                else if (directionX == 1)
                {
                    OnCollision?.Invoke(this, Vector2.right);
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

        bool hitDeath = closestDeathHit - closestHit < -MIN_DISTANCE_TO_DIE;

        if (hitDeath)
        {
            bool isDirectionalDeath = deathCollider.gameObject.GetComponent<DirectionalKiller>() != null;
            OnDeathCollision?.Invoke(this, lastDesiredVelocity.normalized, isDirectionalDeath, deathCollider.gameObject);
        }

        // Find all objects you could collide with
        List<GameObject> hitOtherObjects = new List<GameObject>();
        foreach(RaycastHit2D hit in allOtherHits)
        {
            if (hit.distance - closestHit < -MIN_DISTANCE_TO_DIE)
            {
                hitOtherObjects.Add(hit.collider.gameObject);
            }
        }

        // Remove duplicates
        List<int> IDs = new List<int>();
        foreach(GameObject obj in hitOtherObjects)
        {
            if (IDs.Contains(obj.GetInstanceID())) continue;
            OnOtherCollision?.Invoke(this, lastDesiredVelocity.normalized, obj);
            IDs.Add(obj.GetInstanceID());
        }

        // Leniency (walk up small ledges)
        bool shouldApplyLeniency = false;
        for (int i = 0; i < rayHits.Count; i++)
        {
            bool rayResult = rayHits[i];

            if (i == 0 && rayResult)
            {
                shouldApplyLeniency = true;
            }
            else if (i > 0 && rayResult)
            {
                shouldApplyLeniency = false;
            }
        }
        if (shouldApplyLeniency)
        {
            if (velocity.y < collisionLeniencyStep * collisionLeniencyIterations)
            {
                velocity.y = collisionLeniencyStep * collisionLeniencyIterations;
            }
        }
    }

    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

        Collider2D deathCollider = null;
        float closestHit = Mathf.Infinity;
        float closestDeathHit = Mathf.Infinity;
        List<RaycastHit2D> allOtherHits = new List<RaycastHit2D>();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            RaycastHit2D deathHit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, deathMask);
            allOtherHits.AddRange(Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, otherMask));

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
                    OnCollision?.Invoke(this, Vector2.down);
                    collisions.bottom = true;
                    collisions.top = false;
                }
                else if (directionY == 1)
                {
                    OnCollision?.Invoke(this, Vector2.up);
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

        bool hitDeath = closestDeathHit - closestHit < -MIN_DISTANCE_TO_DIE;

        if (hitDeath)
        {
            bool isDirectionalDeath = deathCollider.gameObject.GetComponent<DirectionalKiller>() != null;
            OnDeathCollision?.Invoke(this, lastDesiredVelocity.normalized, isDirectionalDeath, deathCollider.gameObject);
        }

        // Find all objects you could collide with
        List<GameObject> hitOtherObjects = new List<GameObject>();
        foreach (RaycastHit2D hit in allOtherHits)
        {
            if (hit.distance - closestHit < -MIN_DISTANCE_TO_DIE)
            {
                hitOtherObjects.Add(hit.collider.gameObject);
            }
        }

        // Remove duplicates
        List<int> IDs = new List<int>();
        foreach (GameObject obj in hitOtherObjects)
        {
            if (IDs.Contains(obj.GetInstanceID())) continue;
            OnOtherCollision?.Invoke(this, lastDesiredVelocity.normalized, obj);
            IDs.Add(obj.GetInstanceID());
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

    private Bounds GetStaticCheckBounds()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(STATIC_SKIN_WIDTH * -2);
        return bounds;
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
