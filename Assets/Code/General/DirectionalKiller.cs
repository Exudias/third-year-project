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
