using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(EnergyManager))]
[RequireComponent(typeof(PlayerLogic))]
public class SpiritMovement : MonoBehaviour
{
    [SerializeField] private float maxTopSpeed = 24f;
    [SerializeField] private float minTopSpeed = 0f;
    [SerializeField] private float minTurnDegsPerSec = 90f;
    [SerializeField] private float maxTurnDegsPerSec = 720f;
    [SerializeField] private float wallHitTurnPenaltyTime = 0.2f;
    [SerializeField] private float wallHitTurnPenaltyMultiplier = 0.2f;
    [SerializeField] private float energyDissipationPerSec = 10;
    [SerializeField] private float speedNormalizationTime = 0.3f;

    private float speedNormalizationStep;
    private float speedMultiplier;

    private Vector2 directionOfMovement;
    private Vector2 lastDirection;

    private float timeSinceHitWall;

    private Controller2D controller;
    private EnergyManager energyManager;
    private PlayerLogic playerLogic;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        energyManager = GetComponent<EnergyManager>();
        playerLogic = GetComponent<PlayerLogic>();

        if (directionOfMovement == Vector2.zero)
        {
            directionOfMovement = Vector2.right;
        }

        lastDirection = Vector2.right;
        timeSinceHitWall = Mathf.Infinity;

        speedNormalizationStep = 1 / speedNormalizationTime;
    }

    private void OnEnable()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 desiredDirection = new Vector2(horizontalInput, verticalInput).normalized;

        directionOfMovement = desiredDirection;
        lastDirection = directionOfMovement;

        speedMultiplier = 0;
    }

    private void Update()
    {
        timeSinceHitWall += Time.unscaledDeltaTime;
        speedMultiplier = Mathf.MoveTowards(speedMultiplier, 1, speedNormalizationStep * Time.unscaledDeltaTime);

        float topSpeed = Mathf.Lerp(minTopSpeed, maxTopSpeed, energyManager.GetEnergyPercent());
        float turnDegsPerSec = Mathf.Lerp(minTurnDegsPerSec, maxTurnDegsPerSec, energyManager.GetEnergyPercent());

        energyManager.AddEnergy(-energyDissipationPerSec * Time.unscaledDeltaTime);
        if (energyManager.GetEnergy() == 0)
        {
            playerLogic.Die();
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 desiredDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (desiredDirection == Vector2.zero)
        {
            desiredDirection = lastDirection;
        }

        float desiredAngle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.Atan2(directionOfMovement.y, directionOfMovement.x) * Mathf.Rad2Deg;

        float hitWallMult = timeSinceHitWall < wallHitTurnPenaltyTime ? wallHitTurnPenaltyMultiplier : 1f; 

        currentAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, turnDegsPerSec * hitWallMult * Time.unscaledDeltaTime);

        directionOfMovement = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        if (controller.collisions.bottom || controller.collisions.top)
        {
            directionOfMovement.y *= -1;
            timeSinceHitWall = 0;
            speedMultiplier = 0.5f;
        }
        if (controller.collisions.left || controller.collisions.right)
        {
            directionOfMovement.x *= -1;
            timeSinceHitWall = 0;
            speedMultiplier = 0.5f;
        }

        Vector2 velocity = directionOfMovement * topSpeed * speedMultiplier;

        controller.Move(velocity * Time.unscaledDeltaTime);

        lastDirection = directionOfMovement;
    }
}
