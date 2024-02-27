using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField] private PlayerVisualsManager visualsManager;
    [SerializeField] private Vector2 fromPreviousSpawnPoint;
    [SerializeField] private GameObject bulbDeathObject;
    [SerializeField] private GameObject spiritDeathObject;

    private PlayerFormSwitcher formSwitcher;
    private Controller2D controller;
    private EnergyManager energyManager;

    public delegate void DeathEvent();
    public static event DeathEvent OnPlayerDeath;

    private void Start()
    {
        formSwitcher = GetComponent<PlayerFormSwitcher>();
        controller = GetComponent<Controller2D>();
        energyManager = GetComponent<EnergyManager>();
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
        if (GameManager.IsPlayerDead()) return;
        GameManager.SetPlayerDead(true);
        OnPlayerDeath?.Invoke();
        if (formSwitcher.GetCurrentForm() == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            GameObject deathObj = Instantiate(bulbDeathObject, transform.position, visualsManager.transform.rotation);
            GameManager.MoveObjectToLevelScene(deathObj);
        }
        else
        {
            GameObject deathObj = Instantiate(spiritDeathObject, transform.position, visualsManager.transform.rotation);
            GameManager.MoveObjectToLevelScene(deathObj);
            deathObj.GetComponent<Animator>().Play(0, 0, 1 - energyManager.GetEnergyPercent());
        }
        visualsManager.GetComponent<SpriteRenderer>().enabled = false;
        controller.enabled = false;
        //Camera.main.GetComponent<CameraManager>().SetOrthoSize(1);
        //Camera.main.GetComponent<CameraManager>().SetForcedCameraPosition(transform.position, true, true, true);
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
            Debug.Log(dir);
            if (directionalKiller.ShouldKill(dir))
            {
                Debug.Log("MYEAH");
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
