using UnityEngine;

public class SpikesLogic : MonoBehaviour
{
    private enum SpikeRotations
    {
        Up = 0,
        Down = 180,
        Left = 90,
        Right = 270
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerLogic player = collision.GetComponent<PlayerLogic>();

        if (player != null)
        {
            Controller2D playerController = player.GetComponent<Controller2D>();

            Vector2 lastMovementDirection = playerController.GetLastDesiredVelocity().normalized;
            float spikeRotation = transform.rotation.eulerAngles.z;

            switch ((SpikeRotations)spikeRotation)
            {
                case SpikeRotations.Up:
                    if (lastMovementDirection.y <= 0)
                    {
                        player.Die();
                    }
                    break;
                case SpikeRotations.Down:
                    if (lastMovementDirection.y >= 0)
                    {
                        player.Die();
                    }
                    break;
                case SpikeRotations.Left:
                    if (lastMovementDirection.x >= 0)
                    {
                        player.Die();
                    }
                    break;
                case SpikeRotations.Right:
                    if (lastMovementDirection.x <= 0)
                    {
                        player.Die();
                    }
                    break;
                default:
                    player.Die();
                    break;
            }
        }
    }
}
