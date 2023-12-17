using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField] private Vector2 fromPreviousSpawnPoint;
    [SerializeField] private GameObject bulbDeathObject;

    private PlayerFormSwitcher formSwitcher;
    private Controller2D controller;

    private void Start()
    {
        formSwitcher = GetComponent<PlayerFormSwitcher>();
        controller = GetComponent<Controller2D>();
        if (GameManager.HasCustomSpawn())
        {
            controller.MoveImmediate(GameManager.GetCustomSpawnPoint());
        }
        else if (GameManager.IsFromPrevious())
        {
            controller.MoveImmediate(fromPreviousSpawnPoint);
            GameManager.SetSpawn(fromPreviousSpawnPoint);
        }
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
        if (formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            Instantiate(bulbDeathObject, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    private void OnControllerDeathCollision(Controller2D source, Vector2 dir, bool isDirectionalDeath, GameObject killer)
    {
        if (source != controller) return;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawSphere(fromPreviousSpawnPoint, 0.5f);
    }
}
