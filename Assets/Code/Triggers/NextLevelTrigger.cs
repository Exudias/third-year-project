using UnityEngine;

public class NextLevelTrigger : Trigger
{
    public override void Activate(Collider2D activator)
    {
        bool collisionIsPlayer = activator.gameObject.GetComponent<PlayerLogic>() != null;
        if (collisionIsPlayer)
        {
            GameManager.LoadNextScene();
        }
    }
}
