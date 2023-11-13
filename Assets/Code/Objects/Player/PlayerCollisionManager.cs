using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    private BulbMovement bulbMovement;
    private SpiritMovement spiritMovement;
    private EnergyManager energyManager;
    private PlayerFormSwitcher formSwitcher;
    private Controller2D controller;

    private void Start()
    {
        bulbMovement = GetComponent<BulbMovement>();
        spiritMovement = GetComponent<SpiritMovement>();
        energyManager = GetComponent<EnergyManager>();
        formSwitcher = GetComponent<PlayerFormSwitcher>();
        controller = GetComponent<Controller2D>();
    }

    private void OnEnable()
    {
        Controller2D.OnOtherCollision += OnControllerOtherCollision;
    }

    private void OnDisable()
    {
        Controller2D.OnOtherCollision -= OnControllerOtherCollision;
    }

    private void OnControllerOtherCollision(Vector2 dir, GameObject obj)
    {
        if (obj.GetComponent<EnergyPickupLogic>() != null)
        {
            if (spiritMovement != null && spiritMovement.enabled)
            {
                obj.GetComponent<EnergyPickupLogic>().PlayerCollect(energyManager);
            }
        }
        if (obj.GetComponent<BulbSpawnerLogic>() != null)
        {
            if (spiritMovement != null && spiritMovement.enabled)
            {
                formSwitcher.TurnIntoBulbAt(obj.transform.position);
            }
        }
        if (obj.GetComponent<Trigger>() != null)
        {
            obj.GetComponent<Trigger>().Activate(controller.GetCurrentCollider());
        }    
    }
}
