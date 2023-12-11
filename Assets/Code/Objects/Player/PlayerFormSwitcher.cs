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
    }

    private void InitSpirit()
    {
        EmptyBulbLogic emptyBulb = Instantiate(emptyBulbPrefab, transform.position, Quaternion.identity).GetComponent<EmptyBulbLogic>();
        emptyBulb.SetVelocity(controller.GetLastActualVelocity() / Time.deltaTime);

        currentForm = PlayerForm.Spirit;

        spiritMovement.enabled = true;
        bulbMovement.enabled = false;
        spiritCollider.enabled = true;
        bulbCollider.enabled = false;

        visualsManager.InitSpirit();

        controller.UpdateCollider();

        Camera.main.GetComponent<CameraManager>().ActivateSpiritCamera();
    }
}
