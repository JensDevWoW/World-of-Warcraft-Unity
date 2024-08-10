using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Controls controls;
    Vector2 inputs;
    float rotation = 0;


    Vector3 velocity;
    float gravity = -2, velocityY, terminalVelocity = -25;

    public float baseSpeed = 1, runSpeed = 10, rotateSpeed = 0.4f;

    [SerializeField]
    bool run = true;

    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Locomotion();
    }

    void Locomotion()
    {
        Vector2 inputNormalized = inputs;

        // Rotating
        Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
        transform.eulerAngles = characterRotation;

        //Running and Walking
        float currentSpeed = baseSpeed;
        if (run)
        {
            currentSpeed *= runSpeed;

            if (inputNormalized.y < 0)
                currentSpeed = currentSpeed / 2;
        }
        if (!controller.isGrounded && velocityY > terminalVelocity)
            velocityY += gravity * Time.deltaTime;

        // Apply Inputs
        velocity = (transform.forward * inputNormalized.y + transform.right * inputNormalized.x + Vector3.up * velocityY) * currentSpeed;

        //Moving Controller
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocity.y = 0;
            velocityY = 0;
        }
        
    }

    void GetInputs()
    {
        //Y AXIS
        if (Input.GetKey(controls.forwards))
            inputs.y = 1;

        if (Input.GetKey(controls.backwards))
        {
            if (Input.GetKey(controls.forwards))
                inputs.y = 0;
            else
                inputs.y = -1;
        }


        if (!Input.GetKey(controls.forwards) && !Input.GetKey(controls.backwards))
            inputs.y = 0;

        //X AXIS
        if (Input.GetKey(controls.strafeRight))
            inputs.x = 1;

        if (Input.GetKey(controls.strafeLeft))
        {
            if (Input.GetKey(controls.strafeRight))
                inputs.x = 0;
            else
                inputs.x = -1;
        }


        if (!Input.GetKey(controls.strafeLeft) && !Input.GetKey(controls.strafeRight))
            inputs.x = 0;

        //ROTATE AXIS
        if (Input.GetKey(controls.rotateRight))
            rotation = 1;

        if (Input.GetKey(controls.rotateLeft))
        {
            if (Input.GetKey(controls.rotateRight))
                rotation = 0;
            else
            {
                rotation = -1;
            }

        }


        if (!Input.GetKey(controls.rotateLeft) && !Input.GetKey(controls.rotateRight))
            rotation = 0;

        if (Input.GetKeyDown(controls.walkRun))
            run = !run;
    }
}
