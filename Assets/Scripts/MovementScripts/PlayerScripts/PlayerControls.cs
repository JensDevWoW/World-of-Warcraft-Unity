﻿/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mirror;


public class PlayerControls : NetworkBehaviour
{
    //inputs
    public Controls controls;
    Vector2 inputs;
    [HideInInspector]
    public Vector2 inputNormalized;
    [HideInInspector]
    public float rotation;
    bool run = true, jump;
    [HideInInspector]
    public bool steer, autoRun;
    public LayerMask groundMask;
    public Animator animator;
    public GameObject characterModel;
    public MoveState moveState = MoveState.locomotion;
    private bool wasGrounded;
    //velocity
    Vector3 velocity;
    float gravity = -18, velocityY, terminalVelocity = -25;
    float fallMult;

    private List<int> unitStates = new List<int>();

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [InfoBox("All variables controlling the speed for the player controller is located in this group.")]
    [PropertyTooltip("Changing this will change the walk speed of the player.")]
    public float baseSpeed = 1;

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [PropertyTooltip("Changing this will change the run speed of the player.")]
    public float runSpeed = 4;

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [PropertyTooltip("Changing this will change the speed of player rotating.")]
    public float rotateSpeed = 2;

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [PropertyTooltip("Changing this will change the swim speed of the player.")]
    public float swimSpeed = 2;

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [PropertyTooltip("Changing this will change the mounted speed of the player.")]
    public float mountedSpeed = 1.6f;

    [BoxGroup("Speed Configurations", centerLabel: true)]
    [PropertyTooltip("Changing this will change the flying speed of the player.")]
    public float flyingSpeed = 2.5f;

    float currentSpeed;

    private bool isLanding;  // Flag to control landing animation state

    public GameObject head;

    //ground
    Vector3 forwardDirection, collisionPoint;
    float slopeAngle, directionAngle, forwardAngle, strafeAngle;
    float forwardMult, strafeMult;
    Ray groundRay;
    RaycastHit groundHit;

    //Mounted
    [BoxGroup("Mount Configurations", centerLabel: true)]
    public bool mount;
    [BoxGroup("Mount Configurations", centerLabel: true)]
    public MountType mountType;
    [BoxGroup("Mount Configurations", centerLabel: true)]
    public MountedState mountedState = MountedState.unmounted;

    //Jumping
    [BoxGroup("Jumping Configurations", centerLabel: true)]
    public bool jumping, canJump = true;
    float jumpStartPosY;
    float jumpSpeed, jumpHeight = 3;
    Vector3 jumpDirection;

    //swimming
    [BoxGroup("Swimming Configurations", centerLabel: true)]
    public float swimLevel = 1.25f;
    [BoxGroup("Swimming Configurations", centerLabel: true)]
    public float waterSurface, d_fromWaterSurface;
    [BoxGroup("Swimming Configurations", centerLabel: true)]
    public bool inWater;

    //Debug
    [BoxGroup("Debugging", centerLabel: true)]
    public bool showGroundRay, showMoveDirection, showForwardDirection, showStrafeDirection, showFallNormal, showSwimNormal;

    //references
    CharacterController controller;
    [BoxGroup("References", centerLabel: true)]
    public Transform groundDirection, moveDirection, fallDirection, swimDirection;
    [HideInInspector]
    public CameraController mainCam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = characterModel.GetComponent<Animator>();
    }

    void Update()
    {
        // Ensure we only move the LocalPlayer's character (byproduct of Mirror)
        if (!isLocalPlayer)
            return;

        GetInputs();
        GetSwimDirection();

        // Check if the player is strafing (Q for left, E for right)
        bool isStrafing = Mathf.Abs(inputNormalized.x) > 0;

        // Get the forward direction from the camera
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0; // We only care about the horizontal direction

        // Rotate the player model while strafing
        if (isStrafing)
        {
            // Depending on strafe direction, rotate the character to the correct side
            float rotationDirection = inputNormalized.x > 0 ? 90f : -90f;
            Vector3 strafeDirection = Quaternion.Euler(0, rotationDirection, 0) * cameraForward;

            // Gradually rotate the character to the strafing direction
            characterModel.transform.rotation = Quaternion.Slerp(
                characterModel.transform.rotation,
                Quaternion.LookRotation(strafeDirection),
                Time.deltaTime * rotateSpeed);
        }
        else
        {
            // When strafing stops, rotate the character back to face forward (camera direction)
            characterModel.transform.rotation = Quaternion.Slerp(
                characterModel.transform.rotation,
                Quaternion.LookRotation(cameraForward),
                Time.deltaTime * rotateSpeed);
        }






        // Update animator parameters based on movement
        animator.SetBool("IsRunning", inputNormalized.y > 0 && !jumping);  // Forward movement
        animator.SetBool("IsRunningBack", inputNormalized.y < 0 && !jumping);  // Backward movement
        animator.SetBool("IsStanding", inputNormalized.magnitude == 0 && !jumping);  // Idle
        animator.SetBool("IsFalling", !controller.isGrounded && velocityY < 0);  // Falling
        animator.SetBool("IsJumpingStart", jumping);  // Jumping Start

        if (inWater)
            GetWaterlevel();

        if(mount)
        {
            if (mountType == MountType.ground)
            {
                if(mountedState != MountedState.mounted)
                    mountedState = MountedState.mounted;
            }
            else if(mountType == MountType.flying)
            {
                if (mountedState != MountedState.mountedFlying)
                    mountedState = MountedState.mountedFlying;
            }
        }
        else
        {
            if (mountedState != MountedState.unmounted)
                mountedState = MountedState.unmounted;
        }

        switch (moveState)
        {
            case MoveState.locomotion:
                Locomotion();
                break;

            case MoveState.swimming:
                Swimming();
                break;

            case MoveState.flying:
                Flying();
                break;
        }
    }

    public void AddState(int state)
    {
        if (!unitStates.Contains(state))
            unitStates.Add(state);
    }

    public void RemoveState(int state)
    {
        if (unitStates.Contains(state))
            unitStates.Remove(state);
    }

    public bool HasState(int state)
    {
        return unitStates.Contains(state);
    }

    private void SendMovementUpdate(Vector3 position, Quaternion rotation)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation); // Send rotation data

        if (NetworkClient.isConnected)
        {
            NetworkClient.Send(new OpcodeMessage
            {
                opcode = Opcodes.CMSG_UPDATE_POS,
                payload = writer.ToArray()
            });
        }
    }

    public void OnLandingComplete()
    {
        isLanding = false;
    }
    void Locomotion()
    {
        // Ensure we only move the LocalPlayer's character (byproduct of Mirror)
        if (!isLocalPlayer)
            return;

        GroundDirection();

        // Running and walking logic
        if (controller.isGrounded && slopeAngle <= controller.slopeLimit)
        {
            currentSpeed = baseSpeed;

            if (run)
            {
                currentSpeed *= runSpeed;

                if (mountedState != MountedState.unmounted)
                    currentSpeed *= mountedSpeed;

                if (inputNormalized.y < 0)
                    currentSpeed = currentSpeed / 2;
            }
        }
        else if (!controller.isGrounded || slopeAngle > controller.slopeLimit)
        {
            inputNormalized = Vector2.Lerp(inputNormalized, Vector2.zero, 0.025f);
            currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.025f);
        }

        // Rotating
        Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
        transform.eulerAngles = characterRotation;

        // Jump Logic
        if (jump && controller.isGrounded && slopeAngle <= controller.slopeLimit && !jumping && canJump)
        {
            Jump();
            animator.SetTrigger("Jump");  // Trigger Jump animation
            isLanding = false;  // Interrupt landing animation if jumping
        }

        // Apply gravity if not grounded
        if (!controller.isGrounded)
        {
            isLanding = false;  // Interrupt landing animation if falling
            switch (mountedState)
            {
                case MountedState.unmounted:
                case MountedState.mounted:
                    if (velocityY > terminalVelocity)
                        velocityY += gravity * Time.deltaTime;
                    break;

                case MountedState.mountedFlying:
                    if (Physics.Raycast(groundRay, out groundHit, 0.25f, groundMask))
                    {
                        if (velocityY > terminalVelocity)
                            velocityY += gravity * Time.deltaTime;
                    }
                    else
                        moveState = MoveState.flying;
                    break;
            }
        }
        else if (controller.isGrounded && slopeAngle > controller.slopeLimit)
            velocityY = Mathf.Lerp(velocityY, terminalVelocity, 0.25f);

        // Check WaterLevel
        if (inWater)
        {
            groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
            groundRay.direction = Vector3.down;

            if (d_fromWaterSurface >= swimLevel)
            {
                if (jumping)
                    jumping = false;

                moveState = MoveState.swimming;
            }
        }

        // Applying inputs
        if (!jumping)
        {
            velocity = groundDirection.forward * inputNormalized.y * forwardMult + groundDirection.right * inputNormalized.x * strafeMult; // Applying movement direction inputs
            velocity *= currentSpeed; // Applying current move speed
            velocity += fallDirection.up * (velocityY * fallMult); // Gravity
        }
        else
        {
            velocity = jumpDirection * jumpSpeed + Vector3.up * velocityY;
        }

        // Moving controller
        controller.Move(velocity * Time.deltaTime);

        if (mountedState == MountedState.mountedFlying && jumping)
        {
            float currentJumpHeight = transform.position.y - jumpStartPosY;

            if (currentJumpHeight > 0.5f)
                moveState = MoveState.flying;
        }

        // Detect landing
        if (controller.isGrounded && !wasGrounded)  // Player just landed
        {
            // Stop jumping if grounded
            if (jumping)
            {
                jumping = false;
                canJump = true;  // Allow the player to jump again

                // Reset the jumping state
                animator.ResetTrigger("Jump");

                // Start landing animation logic
                if (inputNormalized.magnitude > 0)
                {
                    animator.SetTrigger("LandAndMoving");  // Trigger landing and moving animation
                }
                else
                {
                    animator.SetTrigger("LandNotMoving");  // Trigger landing not moving animation
                }

                isLanding = true;  // Set landing state
            }

            // Stop gravity if grounded
            velocityY = 0;
        }

        // Update wasGrounded state
        wasGrounded = controller.isGrounded;

        SendMovementUpdate(transform.position, transform.rotation);

        // Set animator parameters based on movement state
        if (!isLanding)  // Only update these if not landing
        {
            animator.SetBool("IsRunning", inputNormalized.y > 0 && !jumping);  // Forward movement
            animator.SetBool("IsRunningBack", inputNormalized.y < 0 && !jumping);  // Backward movement
            animator.SetBool("IsStanding", inputNormalized.magnitude == 0 && !jumping);  // Idle
            animator.SetBool("IsFalling", !controller.isGrounded && velocityY < 0);  // Falling
        }
    }



    void GroundDirection()
    {
        //SETTING FORWARDDIRECTION
        //Setting forwardDirection to controller position
        forwardDirection = transform.position;

        //Setting forwardDirection based on control input.
        if (inputNormalized.magnitude > 0)
            forwardDirection += transform.forward * inputNormalized.y + transform.right * inputNormalized.x;
        else
            forwardDirection += transform.forward;

        //Setting groundDirection to look in the forwardDirection normal
        moveDirection.LookAt(forwardDirection);
        fallDirection.rotation = transform.rotation;
        groundDirection.rotation = transform.rotation;

        //setting ground ray
        groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
        groundRay.direction = Vector3.down;
        
        if(showGroundRay)
            Debug.DrawLine(groundRay.origin, groundRay.origin + Vector3.down * 0.3f, Color.red);

        forwardMult = 1;
        fallMult = 1;
        strafeMult = 1;

        if (Physics.Raycast(groundRay, out groundHit, 0.3f, groundMask))
        {
            //Getting angles
            slopeAngle = Vector3.Angle(transform.up, groundHit.normal);
            directionAngle = Vector3.Angle(moveDirection.forward, groundHit.normal) - 90;

            if (directionAngle < 0 && slopeAngle <= controller.slopeLimit)
            {
                forwardAngle = Vector3.Angle(transform.forward, groundHit.normal) - 90; //Chekcing the forwardAngle against the slope
                forwardMult = 1 / Mathf.Cos(forwardAngle * Mathf.Deg2Rad); //Applying the forward movement multiplier based on the forwardAngle
                groundDirection.eulerAngles += new Vector3(-forwardAngle, 0, 0); //Rotating groundDirection X

                strafeAngle = Vector3.Angle(groundDirection.right, groundHit.normal) - 90; //Checking the strafeAngle against the slope
                strafeMult = 1 / Mathf.Cos(strafeAngle * Mathf.Deg2Rad); //Applying the strafe movement multiplier based on the strafeAngle
                groundDirection.eulerAngles += new Vector3(0, 0, strafeAngle);
            }
            else if(slopeAngle > controller.slopeLimit)
            {
                float groundDistance = Vector3.Distance(groundRay.origin, groundHit.point);

                if(groundDistance <= 0.1f)
                {
                    fallMult = 1 / Mathf.Cos((90 - slopeAngle) * Mathf.Deg2Rad);

                    Vector3 groundCross = Vector3.Cross(groundHit.normal, Vector3.up);
                    fallDirection.rotation = Quaternion.FromToRotation(transform.up, Vector3.Cross(groundCross, groundHit.normal));
                }
            }
        }


        DebugGroundNormals();
    }

    void Jump()
    {
        //set Jumping to true
        if(!jumping)
        {
            jumpStartPosY = transform.position.y;

            jumping = true;
            canJump = false;
        }

        switch(moveState)
        {
            case MoveState.locomotion:
                //Set jump direction and speed
                jumpDirection = (transform.forward * inputs.y + transform.right * inputs.x).normalized;
                jumpSpeed = currentSpeed;

                //set velocity Y
                velocityY = Mathf.Sqrt(-gravity * jumpHeight);
                break;

            case MoveState.swimming:
                //Set jump direction and speed
                jumpDirection = (transform.forward * inputs.y + transform.right * inputs.x).normalized;
                jumpSpeed = swimSpeed;

                //set velocity Y
                velocityY = Mathf.Sqrt(-gravity * jumpHeight * 1.25f);
                break;
        }
    }

    void GetInputs()
    {
        if (HasState(UnitState.UNIT_STATE_STUNNED) || HasState(UnitState.UNIT_STATE_DISORIENTED))
            return; // We cannot do anything while stunned or disoriented

        if (controls.autoRun.GetControlBindingDown())
            autoRun = !autoRun;

        // Forward and backward controls
        if (!HasState(UnitState.UNIT_STATE_ROOTED))
        {
            inputs.y = Axis(controls.forwards.GetControlBinding(), controls.backwards.GetControlBinding());

            if (inputs.y != 0 && !mainCam.autoRunReset)
                autoRun = false;

            if (autoRun)
            {
                inputs.y += Axis(true, false);
                inputs.y = Mathf.Clamp(inputs.y, -1, 1);
            }
        }
        else
        {
            inputs.y = 0; // Disable forward and backward movement when rooted
        }

        // Strafe left and right controls
        if (!HasState(UnitState.UNIT_STATE_ROOTED))
        {
            inputs.x = Axis(controls.strafeRight.GetControlBinding(), controls.strafeLeft.GetControlBinding());
        }
        else
        {
            inputs.x = 0; // Disable strafing when rooted
        }

        // Handling rotation and steering mode
        if (steer && HasState(UnitState.UNIT_STATE_ROOTED))
        {
            // When steering, allow only mouse-based rotation
            rotation = Input.GetAxis("Mouse X") * mainCam.CameraSpeed;
            inputs.x = 0; // Prevent lateral movement
        }
        else if (steer)
        {
            // Normal steering without rooted state
            inputs.x += Axis(controls.rotateRight.GetControlBinding(), controls.rotateLeft.GetControlBinding());
            inputs.x = Mathf.Clamp(inputs.x, -1, 1);
            rotation = Input.GetAxis("Mouse X") * mainCam.CameraSpeed;
        }
        else if (HasState(UnitState.UNIT_STATE_ROOTED))
        {
            // Allow rotation using rotateLeft and rotateRight keys only when not steering
            rotation = Axis(controls.rotateRight.GetControlBinding(), controls.rotateLeft.GetControlBinding());
        }
        else
        {
            // Normal rotation handling when not rooted
            rotation = Axis(controls.rotateRight.GetControlBinding(), controls.rotateLeft.GetControlBinding());
        }

        // Toggle Run
        if (controls.walkRun.GetControlBindingDown())
            run = !run;

        // Jumping - Allow jumping even when rooted
        if (!HasState(UnitState.UNIT_STATE_ROOTED))
            jump = controls.jump.GetControlBinding();

        inputNormalized = inputs.normalized;
    }



    void GetSwimDirection()
    {
        if (steer)
            swimDirection.eulerAngles = transform.eulerAngles + new Vector3(mainCam.tilt.eulerAngles.x, 0, 0);
    }

    void Swimming()
    {
        if(!inWater && !jumping)
        {
            velocityY = 0;
            velocity = new Vector3(velocity.x, velocityY, velocity.z);
            jumpDirection = velocity;
            jumpSpeed = swimSpeed / 2;
            jumping = true;
            moveState = MoveState.locomotion;
        }

        //Rotating
        Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
        transform.eulerAngles = characterRotation;

        //setting ground ray
        groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
        groundRay.direction = Vector3.down;

        if (showGroundRay)
            Debug.DrawLine(groundRay.origin, groundRay.origin + Vector3.down * 0.15f, Color.red);

        if (!jumping && jump && d_fromWaterSurface <= swimLevel)
            Jump();

        if (!jumping)
        {
            velocity = swimDirection.forward * inputNormalized.y + swimDirection.right * inputNormalized.x;

            velocity.y += Axis(jump, controls.sit.GetControlBinding());

            velocity = velocity.normalized;

            velocity *= swimSpeed;

            controller.Move(velocity * Time.deltaTime);

            if (Physics.Raycast(groundRay, out groundHit, 0.15f, groundMask))
            {
                if (d_fromWaterSurface < swimLevel)
                    moveState = MoveState.locomotion;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, float.MinValue, waterSurface - swimLevel), transform.position.z);
            }
        }
        else
        {
            //Jump
            if (velocityY > terminalVelocity)
                velocityY += gravity * Time.deltaTime;

            velocity = jumpDirection * jumpSpeed + Vector3.up * velocityY;

            controller.Move(velocity * Time.deltaTime);

            if (mountedState == MountedState.mountedFlying)
            {
                float currentJumpHeight = transform.position.y - jumpStartPosY;

                if (currentJumpHeight > 0.5f)
                    moveState = MoveState.flying;
            }

            if (Physics.Raycast(groundRay, out groundHit, 0.15f, groundMask))
            {
                if (d_fromWaterSurface < swimLevel)
                    moveState = MoveState.locomotion;
            }

            if (d_fromWaterSurface >= swimLevel)
                jumping = false;
        }
    }

    void Flying()
    {
        if (mountedState == MountedState.unmounted)
            moveState = MoveState.locomotion;

        if(jumping)
        {
            velocityY = 0;
            jumping = false;
        }

        //Rotating
        Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
        transform.eulerAngles = characterRotation;

        //setting ground ray
        groundRay.origin = transform.position + collisionPoint + Vector3.up * 0.05f;
        groundRay.direction = Vector3.down;

        if (showGroundRay)
            Debug.DrawLine(groundRay.origin, groundRay.origin + Vector3.down * 0.15f, Color.red);

        velocity = swimDirection.forward * inputNormalized.y + swimDirection.right * inputNormalized.x;

        velocity.y += Axis(jump, controls.sit.GetControlBinding());

        velocity = velocity.normalized;

        velocity *= runSpeed * flyingSpeed;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
            moveState = MoveState.locomotion;

        if (inWater && d_fromWaterSurface >= swimLevel)
            moveState = MoveState.swimming;
    }

    void GetWaterlevel()
    {
        d_fromWaterSurface = waterSurface - transform.position.y;
    }

    public float Axis(bool pos, bool neg)
    {
        float axis = 0;

        if (pos)
            axis += 1;

        if (neg)
            axis -= 1;

        return axis;
    }

    void DebugGroundNormals()
    {
        Vector3 lineStart = transform.position + Vector3.up * 0.05f;

        if (showMoveDirection)
            Debug.DrawLine(lineStart, lineStart + moveDirection.forward, Color.cyan);

        if (showForwardDirection)
            Debug.DrawLine(lineStart - groundDirection.forward * 0.5f, lineStart + groundDirection.forward * 0.5f, Color.blue);

        if (showStrafeDirection)
            Debug.DrawLine(lineStart - groundDirection.right * 0.5f, lineStart + groundDirection.right * 0.5f, Color.red);

        if (showFallNormal)
            Debug.DrawLine(lineStart, lineStart + fallDirection.up * 0.5f, Color.green);

        if (showSwimNormal)
            Debug.DrawLine(lineStart, lineStart + swimDirection.forward, Color.magenta);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.point.y <= transform.position.y + 0.25f)
        {
            collisionPoint = hit.point;
            collisionPoint = collisionPoint - transform.position;
        }
    }

    public enum MoveState { locomotion, swimming, flying }

    public enum MountedState { unmounted, mounted, mountedFlying }
}

public enum MountType { flying, ground }
