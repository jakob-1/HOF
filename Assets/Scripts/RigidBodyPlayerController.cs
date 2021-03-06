﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyPlayerController : MonoBehaviour {

    public float sideWalkSpeedModifier = 0.9f;
    public float backwardsSpeedModifier = 0.3f;
    public float walkSpeed = 5.0f;
    public float runSpeed = 12.0f;
    private float speed = 5.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 5.0f;
    public Rigidbody rb;
    public bool sprinting;
    private float disToGround;
    public bool falling = false;
    private Player player;
    public bool grounded = true;
    private float sprintStopTimer;
    public bool sliding = false;
    public bool climbing = false;
    public groundDetector groundDetector;


    void Awake()
    {
        rb.freezeRotation = true;
        disToGround = GetComponent<CapsuleCollider>().height / 2;
        player = GetComponent<Player>();
    }

    void FixedUpdate()
    {
        
        grounded = isGrounded();
        if (!grounded)
        {
            grounded = groundDetector.grounded;
        }
        if (Time.time > sprintStopTimer && grounded)
        {
            sprinting = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && player.stamina > 0 && Input.GetAxis("Vertical") > 0)
        {
            sprinting = true;
            maxVelocityChange = runSpeed;
            speed = runSpeed;
        }
        if (grounded && !player.dead && !sliding)
        {
            float speedMod = 1;
            falling = false;
            // Calculate how fast we should be moving
            if (Input.GetAxis("Vertical") < 0)
            {
                speedMod = backwardsSpeedModifier;
            }
            if (climbing)
            {
                speedMod = 0;
            }
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal") * sideWalkSpeedModifier, 0, Input.GetAxis("Vertical") * speedMod);
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (grounded && Input.GetKeyDown("space") && player.stamina > 0)
            {
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                falling = true;
            }
        }

        if (climbing)
        {
            Vector3 position = new Vector3(0, 2, 0);
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += position * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (grounded)
                {
                    transform.position += transform.forward * -backwardsSpeedModifier;
                }
                else
                {
                    transform.position += -position * Time.deltaTime;
                }
                
            }
        }

        //if the don't move then the player isn't sprinting
        if (rb.velocity.x == 0 || rb.velocity.z == 0)
        {
            sprinting = false;
        }

        // We apply gravity manually for more tuning control
        if (falling)
        {
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
        }
        maxVelocityChange = walkSpeed;
        speed = walkSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.stamina > 0)
        {
            sprintStopTimer = Time.time + 0.7f;
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public bool isGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, disToGround + 0.05f);
    }
}
