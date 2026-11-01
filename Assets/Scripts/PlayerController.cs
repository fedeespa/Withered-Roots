using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject chestText;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private Animator animator;

    public float speed = 5f;
    public float jumpHeight = 1f;      // altura real del salto en metros
    public float rotationSpeed = 15f;

    private CharacterController controller;
    private Chest _chest;
    private Vector2 input;
    private bool jumpPressed;
    private float verticalVelocity;

    private bool jumped;
    private float airTimer;
    private float jumpDuration;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpDuration = JumpDurationFor(jumpHeight);
    }

    float JumpDurationFor(float height)
    {
        float g = Physics.gravity.magnitude;
        return 2f * Mathf.Sqrt(2f * height * g) / g; // tiempo total en el aire
    }

    void Update()
    {
        float gravity = Physics.gravity.magnitude;
        bool isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;

            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(2f * jumpHeight * gravity);
                jumpDuration = JumpDurationFor(jumpHeight);
                airTimer = 0f;
                jumped = true;
            }
        }
        jumpPressed = false;

        verticalVelocity -= gravity * Time.deltaTime;

        // Dirección relativa a la cámara
        float yawRad = orbitalFollow.HorizontalAxis.Value * Mathf.Deg2Rad;
        Vector3 camForward = new(Mathf.Sin(yawRad), 0f, Mathf.Cos(yawRad));
        Vector3 camRight = new(Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));
        Vector3 worldMove = camForward * input.y + camRight * input.x;

        Vector3 velocity = worldMove * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Rotar solo si hay movimiento
        if (worldMove.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(worldMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        // Estado después de mover
        if (controller.isGrounded && verticalVelocity <= 0f)
        {
            airTimer = 0f;
            jumped = false;
        }
        else
        {
            airTimer += Time.deltaTime;
        }

        // Si caés de un borde, esperamos un instante para no activar el salto por rebotes
        bool inAir = jumped || airTimer > 0.15f;

        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Clamp01(input.magnitude));
            animator.SetBool("inAir", inAir);
            animator.SetFloat("jumpTime", Mathf.Clamp01(airTimer / jumpDuration));
        }
    }

    public void OnMove(InputValue value) => input = value.Get<Vector2>();

    public void OnJump(InputValue value)
    {
        if (value.isPressed) jumpPressed = true;
    }

    public void OnInteract(InputValue value)
    {
        if (_chest == null) return;
        _chest.Open();
        _chest = null;
        if (chestText != null) chestText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene("CombatScene");
        }
        else if (other.CompareTag("Chest"))
        {
            _chest = other.GetComponentInParent<Chest>();
            if (chestText != null) chestText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chest") && other.GetComponentInParent<Chest>() == _chest)
        {
            _chest = null;
            if (chestText != null) chestText.SetActive(false);
        }
    }
}