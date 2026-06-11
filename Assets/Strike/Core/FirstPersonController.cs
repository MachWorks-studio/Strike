using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Firearm")]
    [SerializeField] private GameObject firearm;

    [SerializeField] private Gun gun;
    [Header("Movement Speeds")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float walkSpeedMultiplier = 0.2f;
    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float upDownRange = 80.0f;
    [Header("Crouch Parameters")]
    private float minCrouchHeight = -0.9f;
    private float maxCrouchHeight = 0.5f;
    [SerializeField] private float scrollStep = 0.1f;
    [SerializeField] private float heightLerpSpeed = 10f;
    [SerializeField] private Transform mainCameraTransform;
    private float targetHeight;
    private float currentHeight;

    private CharacterController characterController;
    private Camera mainCamera;
    private Vector3 currentMovement;
    private PlayerInput playerInput;
    private Vector3 defaultFirearmPosition;
    // Input Values
    private Vector2 playerMoveInput;
    private bool playerJumpInput;
    private bool playerSprintInput;
    private float verticalRotation;
    private Vector2 mouseLookInput;
    private bool playerWalkInput;
    private bool playerAimInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        targetHeight = mainCameraTransform.localPosition.y;
        currentHeight = mainCameraTransform.localPosition.y;
        defaultFirearmPosition = firearm.transform.localPosition;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gun.SetEventCamera(mainCamera);
    }
    
    private void Update()
    {
        playerMoveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        playerJumpInput = playerInput.actions["Jump"].IsPressed();
        playerSprintInput = playerInput.actions["Sprint"].IsPressed();
        playerWalkInput = playerInput.actions["Walk"].IsPressed();

        Look();
        MoveCharacter(currentMovement);
        Gravity();
        ReadScrollInput();
        SmoothHeight();
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
            speed = moveSpeed * sprintMultiplier;
        }
        else
        {
            if (playerWalkInput == true)
            {
                speed = moveSpeed * walkSpeedMultiplier;
            }
            else
            {
                speed = moveSpeed;
            }
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

    void OnAim(InputValue value)
    {
        if (value.isPressed)
        {
            firearm.transform.localPosition = new Vector3(0, -0.148f, firearm.transform.localPosition.z);
        }
        else
        {
            firearm.transform.localPosition = defaultFirearmPosition;
        }
    }

    void OnSprint(InputValue value)
    {
        playerSprintInput = value.isPressed;
    }

    void OnWalk(InputValue value)
    {
        playerWalkInput = value.isPressed;
    }

    void ReadScrollInput()
    {
        float scroll = playerInput.actions["CrouchScroll"].ReadValue<float>();
        if (scroll == 0) return;
        targetHeight += scroll > 0 ? scrollStep : -scrollStep;
        targetHeight = Mathf.Clamp(targetHeight, minCrouchHeight, maxCrouchHeight);
    }

    void SmoothHeight()
    {
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * heightLerpSpeed);
        ApplyHeight(currentHeight);
    }
    void ApplyHeight(float height)
    {
        mainCameraTransform.localPosition = new Vector3(mainCameraTransform.localPosition.x, height, mainCameraTransform.localPosition.z);
    }
}
