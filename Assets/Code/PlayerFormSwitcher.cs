using UnityEngine;

[RequireComponent(typeof(SpiritMovement))]
[RequireComponent(typeof(BulbMovement))]
public class PlayerFormSwitcher : MonoBehaviour
{
    public enum PlayerForm
    {
        Bulb,
        Spirit
    }

    [SerializeField] private PlayerForm currentForm = PlayerForm.Bulb;
    [SerializeField] private PlayerVisualsManager visualsManager;

    private BulbMovement bulbMovement;
    private SpiritMovement spiritMovement;

    private void Start()
    {
        bulbMovement = GetComponent<BulbMovement>();
        spiritMovement = GetComponent<SpiritMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
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

    public PlayerForm GetCurrentForm()
    {
        return currentForm;
    }

    private void InitBulb()
    {
        currentForm = PlayerForm.Bulb;
        bulbMovement.enabled = true;
        spiritMovement.enabled = false;
        visualsManager.InitBulb();
    }

    private void InitSpirit()
    {
        currentForm = PlayerForm.Spirit;
        spiritMovement.enabled = true;
        bulbMovement.enabled = false;
        visualsManager.InitSpirit();
    }
}
