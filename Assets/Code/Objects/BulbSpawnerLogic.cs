using UnityEngine;

public class BulbSpawnerLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SpiritMovement playerSpirit = collision.GetComponent<SpiritMovement>();

            if (playerSpirit != null && playerSpirit.enabled)
            {
                PlayerFormSwitcher playerFormSwitcher = collision.GetComponent<PlayerFormSwitcher>();

                playerFormSwitcher?.TurnIntoBulbAt(transform.position);
            }
        }
    }
}
