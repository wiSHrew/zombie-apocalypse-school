using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController characterController;
    private JumpTracker jumpTracker;
    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        jumpTracker = GetComponent<JumpTracker>();
        if (cameraTransform == null)
        {
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                cameraTransform = playerCamera.transform;
            }
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Player Controls requires a camera assigned or parented to the player.", this);
            enabled = false;
            return;
        }

        pitch = cameraTransform.localEulerAngles.x;
        if (pitch > 180f)
        {
            pitch -= 360f;
        }
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState == CursorLockMode.Locked && mouse != null)
        {
            Vector2 lookInput = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * lookInput.x);
            pitch = Mathf.Clamp(pitch - lookInput.y, -maxLookAngle, maxLookAngle);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        Vector2 moveInput = Vector2.zero;
        bool sprinting = false;
        bool jumpPressed = false;

        if (keyboard != null)
        {
            moveInput.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
            moveInput.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
            sprinting = keyboard.leftShiftKey.isPressed;
            jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
            if (jumpPressed)
            {
                if (jumpTracker != null)
                {
                    jumpTracker.BeginJump();
                }

                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        movement = movement * speed + Vector3.up * verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }
}
