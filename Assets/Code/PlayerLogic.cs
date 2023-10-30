using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private PlayerFormSwitcher formSwitcher;

    private void Start()
    {
        formSwitcher = GetComponent<PlayerFormSwitcher>();
    }

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
            if (killer.GetComponent<FormKiller>() != null)
            {
                bool shouldKillForm = formSwitcher != null && formSwitcher.GetCurrentForm() == killer.GetComponent<FormKiller>().GetForm();
                if (shouldKillForm)
                {
                    Die();
                }
            }
            else
            {
                Die();
            }
        }
        else
        {
            DirectionalKiller directionalKiller = killer.GetComponent<DirectionalKiller>();
            if (directionalKiller.ShouldKill(dir))
            {
                if (killer.GetComponent<FormKiller>() != null)
                {
                    bool shouldKillForm = formSwitcher != null && formSwitcher.GetCurrentForm() == killer.GetComponent<FormKiller>().GetForm();
                    if (shouldKillForm)
                    {
                        Die();
                    }
                }
                else
                {
                    Die();
                }
            }
        }
    }
}
