using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collisionIsPlayer = collision.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            GameManager.LoadNextScene();
        }
    }
}
