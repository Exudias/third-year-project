using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalKiller : MonoBehaviour
{
    private enum DirectionRotations
    {
        Up = 0,
        Down = 180,
        Left = 90,
        Right = 270
    }

    // This is to catch players that don't move inside spikes
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerLogic player = collision.GetComponent<PlayerLogic>();

        if (player != null)
        {
            Controller2D playerController = player.GetComponent<Controller2D>();

            Vector2 lastMovementDirection = playerController.GetLastDesiredVelocity().normalized;

            if (ShouldKill(lastMovementDirection))
            {
                player.Die();
            }
        }
    }

    public bool ShouldKill(Vector2 collisionDirection)
    {
        float dangerRotation = transform.rotation.eulerAngles.z;

        // Yes it could be smaller, but would it be readable?
        switch ((DirectionRotations)dangerRotation)
        {
            case DirectionRotations.Up:
                if (collisionDirection.y <= 0)
                {
                    return true;
                }
                break;
            case DirectionRotations.Down:
                if (collisionDirection.y >= 0)
                {
                    return true;
                }
                break;
            case DirectionRotations.Left:
                if (collisionDirection.x >= 0)
                {
                    return true;
                }
                break;
            case DirectionRotations.Right:
                if (collisionDirection.x <= 0)
                {
                    return true;
                }
                break;
            default:
                return true;
        }
        return false;
    }
}
