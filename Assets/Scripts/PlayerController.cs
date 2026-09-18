using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 0.25f;

    private CharacterController controller;
    private Vector2 input;
    private bool jumpPressed;
    private float verticalVelocity;

    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

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
            verticalVelocity = (float)Math.Sqrt(jumpHeight * Physics.gravity.magnitude);
            jumpPressed = false;
        }

        verticalVelocity += -Physics.gravity.magnitude * Time.deltaTime;

        float yawRad = orbitalFollow.HorizontalAxis.Value * Mathf.Deg2Rad;

        Vector3 camForward = new(Mathf.Sin(yawRad), 0f, Mathf.Cos(yawRad));
        Vector3 camRight = new(Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));

        Vector3 worldMove = camForward * input.y + camRight * input.x;

        var move = new Vector3(worldMove.x, verticalVelocity / speed, worldMove.z);

        controller.Move(speed * Time.deltaTime * move);

        transform.rotation = Quaternion.Euler(0, orbitalFollow.HorizontalAxis.Value, 0);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene("CombatScene");
        }
    }
}
