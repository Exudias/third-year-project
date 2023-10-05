using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float topSpeed = 8f;
    [SerializeField] private float acceleration = 100;
    [SerializeField] private float deceleration = 160;
    [SerializeField] private float airControlMult = 0.7f;
    [Header("Vertical Movement")]
    [SerializeField] private float maxJumpHeight = 3;
    [SerializeField] private float minJumpHeight = 0.5f;
    [SerializeField] private float timeToJumpApex = 0.3f;
    [SerializeField] private float jumpBufferLeniency = 0.1f;
    [SerializeField] private float coyoteJumpLeniency = 0.1f;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector2 velocity;
    private float coyoteTime;
    private float timeSinceJumpPressed;
    private bool jumping;

    private Controller2D controller;

    private void Start()
    {
        controller = GetComponent<Controller2D>();

        CalculateGravityAndJumpVelocity();
        timeSinceJumpPressed = Mathf.Infinity;
        coyoteTime = Mathf.Infinity;
    }

    private void Update()
    {
        coyoteTime += Time.deltaTime;
        timeSinceJumpPressed += Time.deltaTime;

        if (controller.collisions.top || controller.collisions.bottom)
        {
            velocity.y = 0;
        }

        if (controller.collisions.bottom)
        {
            coyoteTime = 0;
            jumping = false;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeSinceJumpPressed = 0;
        }

        if (jumping && !Input.GetKey(KeyCode.Space))
        {
            EndJump();
        }

        if (coyoteTime <= coyoteJumpLeniency && timeSinceJumpPressed <= jumpBufferLeniency)
        {
            Jump();
        }

        float targetVelocityX = horizontalInput * topSpeed;
        float accelToUse = (targetVelocityX * horizontalInput <= 0) ? deceleration : acceleration;
        accelToUse *= controller.collisions.bottom ? 1 : airControlMult;

        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, accelToUse * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = maxJumpVelocity;
        timeSinceJumpPressed = Mathf.Infinity;
        coyoteTime = Mathf.Infinity;
        jumping = true;
    }

    private void EndJump()
    {
        velocity.y = Mathf.Min(minJumpVelocity, velocity.y);
    }

    private void CalculateGravityAndJumpVelocity()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
}
