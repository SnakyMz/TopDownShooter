using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] LayerMask aimLayer;
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 3f;

    [Header("Weapon Settings")]
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject shotgun;
    [SerializeField] GameObject autogun;
    [SerializeField] GameObject rifle;

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
        animator = GetComponent<Animator>();
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
        ToggleWeapon();
        Shoot();
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

    void Shoot()
    {
        if (controls.Attack.WasPerformedThisFrame())
        {
            animator.SetTrigger("Shoot");
        }
    }

    void ToggleWeapon()
    {
        if (controls.Weapon1.WasPerformedThisFrame()) SwitchWeapon(pistol);
        else if (controls.Weapon2.WasPerformedThisFrame()) SwitchWeapon(shotgun);
        else if (controls.Weapon3.WasPerformedThisFrame()) SwitchWeapon(autogun);
        else if (controls.Weapon4.WasPerformedThisFrame()) SwitchWeapon(rifle);
    }

    void SwitchWeapon(GameObject gun)
    {
        pistol.SetActive(false);
        shotgun.SetActive(false);
        autogun.SetActive(false);
        rifle.SetActive(false);
        gun.SetActive(true);
    }
}
