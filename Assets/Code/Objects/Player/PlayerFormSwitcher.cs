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

    public delegate void FormSwitchEvent();
    public static event FormSwitchEvent OnSwitchToBulb;
    public static event FormSwitchEvent OnSwitchToSpirit;

    private void Start()
    {
        bulbMovement = GetComponent<BulbMovement>();
        spiritMovement = GetComponent<SpiritMovement>();
        controller = GetComponent<Controller2D>();
        input = InputManager.instance;
    }

    private void Update()
    {
        // go spirit from bulb, but to return you must collide with a bulb, so no on-demand
        if ((input.IsDown(KeyCode.LeftShift) || input.IsDown(KeyCode.RightShift)) && currentForm == PlayerForm.Bulb)
        {
            ToggleForm();
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
        controller.MoveImmediate(bulbPosition);
        InitBulb();
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

        Camera.main.GetComponent<CameraManager>().ActivateBulbCamera();
        OnSwitchToBulb?.Invoke();
    }

    Vector3 EMPTY_BULB_SPAWN_OFFSET = new Vector2(0, 0.25f);
    private void InitSpirit()
    {
        // offset only if off the ground, otherwise put on ground (mainly for super jump)
        Vector3 offset = controller.collisions.bottom ? Vector3.zero : EMPTY_BULB_SPAWN_OFFSET;
        EmptyBulbLogic emptyBulb = Instantiate(emptyBulbPrefab, transform.position + offset, Quaternion.identity).transform.GetChild(0).GetComponent<EmptyBulbLogic>();
        emptyBulb.SetVelocity(controller.GetLastActualVelocity() / Time.deltaTime);
        emptyBulb.SetGravity(bulbMovement.GetGravity());
        emptyBulb.SetTerminalVelocity(bulbMovement.GetTerminalVelocity());
        emptyBulb.SetDeceleration(bulbMovement.GetDeceleration());
        emptyBulb.SetDestroyOnSolid(!controller.collisions.bottom);
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
}
