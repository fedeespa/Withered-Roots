using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector2 input;
    private bool jumpPressed;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (jumpPressed && isGrounded)
        {
            verticalVelocity = (float)Math.Sqrt(jumpHeight * -2f * -Physics.gravity.magnitude);
            jumpPressed = false;
        }

        verticalVelocity += -Physics.gravity.magnitude * Time.deltaTime;

        var move = new Vector3(input.x, 0, input.y)
        {
            y = verticalVelocity / speed
        };

        controller.Move(speed * Time.deltaTime * move);
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }

    OnCollisionEnter
}
