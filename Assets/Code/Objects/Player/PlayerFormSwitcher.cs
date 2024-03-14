using UnityEngine;

[RequireComponent(typeof(SpiritMovement))]
[RequireComponent(typeof(BulbMovement))]
[RequireComponent(typeof(Controller2D))]
public class PlayerFormSwitcher : MonoBehaviour
{
    public enum PlayerForm
    {
        Bulb,
        Spirit
    }

    [SerializeField] private PlayerForm currentForm = PlayerForm.Bulb;
    [SerializeField] private PlayerVisualsManager visualsManager;

    [SerializeField] private BoxCollider2D bulbCollider;
    [SerializeField] private BoxCollider2D spiritCollider;

    [SerializeField] private GameObject emptyBulbPrefab;
     
    private BulbMovement bulbMovement;
    private SpiritMovement spiritMovement;
    private Controller2D controller;
    private InputManager input;
    private EnergyManager energyManager;

    public delegate void FormSwitchEvent();
    public static event FormSwitchEvent OnSwitchToBulb;
    public static event FormSwitchEvent OnSwitchToSpirit;
    public static event FormSwitchEvent OnFailedSwitchToSpirit;
    public static event FormSwitchEvent OnInitAsBulb;
    public static event FormSwitchEvent OnInitAsSpirit;

    private bool canTransformToSpirit = true;

    private void Start()
    {
        bulbMovement = GetComponent<BulbMovement>();
        spiritMovement = GetComponent<SpiritMovement>();
        controller = GetComponent<Controller2D>();
        energyManager = GetComponent<EnergyManager>();
        input = InputManager.instance;
        canTransformToSpirit = true;
        if (currentForm == PlayerForm.Bulb)
        {
            OnInitAsBulb?.Invoke();
        }
        else if (currentForm == PlayerForm.Spirit)
        {
            OnInitAsSpirit?.Invoke();
        }
    }

    private Vector2 lastHorizontalDirection = Vector2.right;

    private void Update()
    {
        if (GameManager.IsPlayerDead()) return;

        // Update last dir
        float horizontalInput = input.GetHorizontalRaw();
        float verticalInput = input.GetVerticalRaw();

        Vector2 desiredDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (desiredDirection.x != 0)
        {
            lastHorizontalDirection = Vector2.right * desiredDirection.x;
        }

        // go spirit from bulb, but to return you must collide with a bulb, so no on-demand
        bool pressedTransform = input.IsDown(KeyCode.LeftShift) || input.IsDown(KeyCode.RightShift);
        bool stateIsGood = !GameManager.IsLoadingScene() && !GameManager.IsGamePaused() && !GameManager.IsPlayerDead();
        if (canTransformToSpirit)
        {
            if (pressedTransform && currentForm == PlayerForm.Bulb && stateIsGood)
            {
                ToggleForm();
            }
        }
        else
        {
            if (pressedTransform)
            {
                OnFailedSwitchToSpirit?.Invoke();
            }
        }
    }

    private void ToggleForm()
    {
        if (currentForm == PlayerForm.Bulb)
        {
            InitSpirit();
        }
        else if (currentForm == PlayerForm.Spirit)
        {
            InitBulb();
        }
        else
        {
            throw new System.Exception("Invalid player form to switch from!");
        }
    }

    public void TurnIntoBulbAt(Vector2 bulbPosition)
    {
        InitBulb();
        controller.MoveImmediate(bulbPosition);
        OnSwitchToBulb?.Invoke();
    }

    public PlayerForm GetCurrentForm()
    {
        return currentForm;
    }

    private void InitBulb()
    {
        currentForm = PlayerForm.Bulb;

        bulbMovement.enabled = true;
        spiritMovement.enabled = false;
        bulbCollider.enabled = true;
        spiritCollider.enabled = false;

        visualsManager.InitBulb();

        controller.UpdateCollider();

        energyManager?.ResetEnergy();

        Camera.main.GetComponent<CameraManager>().ActivateBulbCamera();
    }

    private void InitSpirit()
    {
        EmptyBulbLogic emptyBulb = Instantiate(emptyBulbPrefab, transform.position, Quaternion.identity).transform.GetChild(0).GetComponent<EmptyBulbLogic>();
        emptyBulb.SetVelocity(controller.GetLastActualVelocity() / Time.unscaledDeltaTime);
        emptyBulb.SetGravity(bulbMovement.GetGravity());
        emptyBulb.SetTerminalVelocity(bulbMovement.GetTerminalVelocity());
        emptyBulb.SetDeceleration(bulbMovement.GetDeceleration());
        emptyBulb.SetDestroyOnSolid(!controller.collisions.bottom);

        // offset sprite only if off the ground, otherwise put on ground
        Vector2 EMPTY_BULB_SPAWN_OFFSET = new Vector2(0, 0.25f);
        Vector2 offset = controller.collisions.bottom ? Vector2.zero : EMPTY_BULB_SPAWN_OFFSET;
        emptyBulb.SetSpriteOffset(offset);

        GameManager.MoveObjectToLevelScene(emptyBulb.transform.parent.gameObject);

        currentForm = PlayerForm.Spirit;

        spiritMovement.enabled = true;
        bulbMovement.enabled = false;
        spiritCollider.enabled = true;
        bulbCollider.enabled = false;

        visualsManager.InitSpirit();

        controller.UpdateCollider();

        Camera.main.GetComponent<CameraManager>().ActivateSpiritCamera();
        OnSwitchToSpirit?.Invoke();
    }

    public Vector2 GetLastHorizontalVelocity() => lastHorizontalDirection;

    public void SetCanTransform(bool newVal)
    {
        canTransformToSpirit = newVal;
    }
}
