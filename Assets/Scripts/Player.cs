using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] Transform aim;
    [SerializeField] LayerMask aimLayer;
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 3f;

    Controls.PlayerActions controls;
    CharacterController controller;
    Animator animator;

    Vector2 moveInput;
    Vector2 aimInput;
    Vector3 velocity;
    float verticalVelocity;
    Vector3 lookDirection;

    bool isRunning;

    void Awake()
    {
        controls = PlayerControl.Controls.Player;
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Movement();
    }

    // Getting Inputs
    void GetInputs()
    {
        moveInput = controls.Move.ReadValue<Vector2>();
        aimInput = controls.Aim.ReadValue<Vector2>();
        isRunning = controls.Sprint.IsPressed();
    }

    // Input Actions
    void Movement()
    {
        // Aim and Rotation
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayer))
        {
            lookDirection = hit.point - transform.position;
            lookDirection.y = 0;
            lookDirection.Normalize();

            transform.forward = lookDirection;
            aim.position = hit.point + Vector3.up;
        }

        // Gravity
        if (!controller.isGrounded) verticalVelocity += Physics.gravity.y;
        else verticalVelocity = -0.5f;

        // Movement
        float moveSpeed = isRunning ? runSpeed : walkSpeed;
        velocity = new(moveInput.x * moveSpeed, verticalVelocity, moveInput.y * moveSpeed);
        if (moveInput.magnitude > 0)
        {
            controller.Move(velocity * Time.deltaTime);
        }

        float x = Vector3.Dot(velocity.normalized, transform.right);
        float y = Vector3.Dot(velocity.normalized, transform.forward);
        animator.SetFloat("Side", isRunning ? x * 2 : x, 0.1f, Time.deltaTime);
        animator.SetFloat("Forward", isRunning ? y * 2 : y, 0.1f, Time.deltaTime);
    }
}
