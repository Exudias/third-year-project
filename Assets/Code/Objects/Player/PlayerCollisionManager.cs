using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    private BulbMovement bulbMovement;
    private SpiritMovement spiritMovement;
    private EnergyManager energyManager;
    private PlayerFormSwitcher formSwitcher;
    private Controller2D controller;

    private void Awake()
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
        PlayerFormSwitcher.OnSwitchToSpirit += OnSpirit;
        PlayerFormSwitcher.OnInitAsSpirit += OnSpirit;
        PlayerFormSwitcher.OnSwitchToBulb += OnBulb;
        PlayerFormSwitcher.OnInitAsBulb += OnBulb;
    }

    private void OnDisable()
    {
        Controller2D.OnOtherCollision -= OnControllerOtherCollision;
        PlayerFormSwitcher.OnSwitchToSpirit -= OnSpirit;
        PlayerFormSwitcher.OnInitAsSpirit -= OnSpirit;
        PlayerFormSwitcher.OnSwitchToBulb -= OnBulb;
        PlayerFormSwitcher.OnInitAsBulb -= OnBulb;
    }

    private void OnBulb()
    {
        controller.AddToSolidCollisions(LayerMask.GetMask("BulbSolid"));
        controller.RemoveFromSolidCollisions(LayerMask.GetMask("SpiritSolid"));
    }

    private void OnSpirit()
    {
        controller.AddToSolidCollisions(LayerMask.GetMask("SpiritSolid"));
        controller.RemoveFromSolidCollisions(LayerMask.GetMask("BulbSolid"));
    }

    private void OnControllerOtherCollision(Controller2D source, Vector2 dir, GameObject obj)
    {
        if (source != controller) return;
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
                BulbSpawnerLogic bulbSpawner = obj.GetComponent<BulbSpawnerLogic>();
                if (!bulbSpawner.IsEmpty())
                {
                    bulbSpawner.HandlePickup();
                    formSwitcher.TurnIntoBulbAt(bulbSpawner.GetSpawnPoint());
                }
            }
        }
        if (obj.GetComponent<EmptyBulbLogic>() != null)
        {
            if (spiritMovement != null && spiritMovement.enabled)
            {
                EmptyBulbLogic emptyBulb = obj.GetComponent<EmptyBulbLogic>();
                if (!emptyBulb.HasCooldown())
                {
                    formSwitcher.TurnIntoBulbAt(emptyBulb.transform.position);
                    emptyBulb.Collect();
                }
            }
        }
        if (obj.GetComponent<Trigger>() != null)
        {
            obj.GetComponent<Trigger>().Activate(controller.GetCurrentCollider());
        }    
    }
}
