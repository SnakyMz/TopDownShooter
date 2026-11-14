using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] LayerMask aimLayer;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 500f;


    PlayerInput playerInput;
    CharacterController controller;

    Vector2 moveInput;
    Vector2 aimInput;
    Vector3 velocity;
    float verticalVelocity;
    Vector3 lookDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    // Getting Inputs
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
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

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        // Gravity
        if (!controller.isGrounded) verticalVelocity += Physics.gravity.y;
        else verticalVelocity = -0.5f;

        // Movement
        velocity = new(moveInput.x * moveSpeed, verticalVelocity, moveInput.y * moveSpeed);
        if (moveInput.magnitude > 0)
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
