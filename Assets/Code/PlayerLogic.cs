using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private void OnEnable()
    {
        Controller2D.OnDeathCollision += OnControllerDeathCollision;
    }

    private void OnDisable()
    {
        Controller2D.OnDeathCollision -= OnControllerDeathCollision;
    }

    public void Die()
    {
        GameManager.ResetScene();
    }

    private void OnControllerDeathCollision(Vector2 dir, bool isDirectionalDeath, GameObject killer)
    {
        if (!isDirectionalDeath)
        {
            Die();
        }
        else
        {
            DirectionalKiller directionalKiller = killer.GetComponent<DirectionalKiller>();
            if (directionalKiller.ShouldKill(dir))
            {
                Die();
            }
        }
    }
}
