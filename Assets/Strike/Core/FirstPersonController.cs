using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class FirstPersonController : MonoBehaviour
{
    [Header("Firearm")]
    [SerializeField] private GameObject firearm;
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float upDownRange = 80.0f;

    private CharacterController characterController;
    private Camera mainCamera;
    private Vector3 currentMovement;
    private PlayerInput playerInput;
    // Input Values
    private Vector2 playerMoveInput;
    private bool playerJumpInput;
    private bool playerSprintInput;
    private float verticalRotation;
    private Vector2 mouseLookInput;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    
    private void Update()
    {
        playerMoveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        playerJumpInput = playerInput.actions["Jump"].IsPressed();
        playerSprintInput = playerInput.actions["Sprint"].IsPressed();

        Look();
        MoveCharacter(currentMovement);
        Gravity();
    }
    void OnMove(InputValue value)
    {
        //Debug.Log("On Move happening");
    }
    void MoveCharacter(Vector3 movementVector)
    {
        float speed;
        if (playerSprintInput == true)
        {
            speed = walkSpeed * sprintMultiplier;
        }
        else
        {
            speed = walkSpeed;
        }
        Vector3 inputDirection = new Vector3(playerMoveInput.x, 0f, playerMoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;
        characterController.Move(movementVector * Time.deltaTime);
    }

    void Gravity()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerJumpInput == true)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }
    void OnLook(InputValue value)
    {
        mouseLookInput = value.Get<Vector2>();
    }

    void Look()
    {
        float mouseXRotation = mouseLookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= mouseLookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void OnSprint(InputValue value)
    {
        playerSprintInput = value.isPressed;
    }
}
