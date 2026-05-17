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
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField] private float upDownRange = 80.0f;

    private CharacterController characterController;
    private Camera mainCamera;
    private Vector3 currentMovement;
    private float verticalRotation;
    private float sprintInput;
    private Vector2 mouseLookInput;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    
    private void Update()
    {
        Look();
        MoveCharacter(currentMovement);
    }
    void OnMove(InputValue value)
    {
        float speed = walkSpeed;
        Vector2 moveInput = value.Get<Vector2>();
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;
    }
    void MoveCharacter(Vector3 movementVector)
    {
        characterController.Move(movementVector * Time.deltaTime);
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

    // SPRINT IS NOT IMPLEMENTED YET
    /*
    void OnSprint(InputValue value)
    {
        sprintInput = value.Get<float>();
    }
    */

    // JUMP IS NOT IMPLEMENTED YET
    /*
    void OnJump()
    {
        currentMovement.y = jumpForce;
        Debug.Log("Jumped");
    }
    */
}
