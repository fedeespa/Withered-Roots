using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject chestText;
    private Chest _chest;

    public float speed = 5f;
    public float jumpHeight = 0.25f;

    private CharacterController controller;
    private Vector2 input;
    private bool jumpPressed;
    private float verticalVelocity;

    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    [SerializeField] private Animator animator;
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
        
        if (worldMove.sqrMagnitude > 0.01f)
        {
            
            float targetAngle = Mathf.Atan2(worldMove.x, worldMove.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
        float moveAmount = Mathf.Clamp01(input.magnitude);
        if (animator != null)
        {
            animator.SetFloat("speed", moveAmount);
        }
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

    public void OnInteract(InputValue value)
    {
        if (_chest != null)
        {
            _chest.Open();
            _chest = null;
            chestText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene("CombatScene");
            return;
        }

        if (other.CompareTag("Chest"))
        {
            _chest = other.GetComponentInParent<Chest>();
            chestText.SetActive(true);
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            _chest = null;
            chestText.SetActive(false);
            return;
        }
    }
}
