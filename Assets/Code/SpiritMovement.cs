using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class SpiritMovement : MonoBehaviour
{
    [SerializeField] private float maxTopSpeed = 24f;
    [SerializeField] private float minTopSpeed = 12f;
    [SerializeField] private float turnDegsPerSec = 180f;
    [SerializeField] private float energyDissipationPerSec = 10;

    private Vector2 directionOfMovement;
    private Vector2 lastDirection;

    private Controller2D controller;
    private EnergyManager energyManager;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
        energyManager = EnergyManager.Instance;

        if (directionOfMovement == Vector2.zero)
        {
            directionOfMovement = Vector2.right;
        }

        lastDirection = Vector2.right;
    }

    private void Update()
    {
        float topSpeed = Mathf.Lerp(minTopSpeed, maxTopSpeed, energyManager.GetEnergyPercent());
        energyManager.AddEnergy(-energyDissipationPerSec * Time.deltaTime);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 desiredDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (desiredDirection == Vector2.zero)
        {
            desiredDirection = lastDirection;
        }

        float desiredAngle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.Atan2(directionOfMovement.y, directionOfMovement.x) * Mathf.Rad2Deg;
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, turnDegsPerSec * Time.deltaTime);

        directionOfMovement = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        if (controller.collisions.bottom || controller.collisions.top)
        {
            directionOfMovement.y *= -1;
        }
        if (controller.collisions.left || controller.collisions.right)
        {
            directionOfMovement.x *= -1;
        }

        Vector2 velocity = directionOfMovement * topSpeed;

        controller.Move(velocity * Time.deltaTime);

        lastDirection = directionOfMovement;
    }
}
