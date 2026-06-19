using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 150f;

    private CharacterController characterController;

    private float verticalRotation;

    private Camera playerCamera;

    private bool canMove = true;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        playerCamera = GetComponentInChildren<Camera>();

        LockCursor();
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement =
            transform.right * horizontal +
            transform.forward * vertical;

        characterController.Move(
            movement * moveSpeed * Time.deltaTime
        );
    }

    private void HandleMouseLook()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -80f,
            80f
        );

        playerCamera.transform.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );

        transform.Rotate(
            Vector3.up * mouseX
        );
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (enabled)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}