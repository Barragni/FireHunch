using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 150f;

    private CharacterController characterController;
    private Camera playerCamera;

    private float verticalRotation;

    private bool canMove;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        SetMovementEnabled(false);
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }

    public IEnumerator LookAtTarget(Transform target, float duration)
    {
        canMove = false;

        Vector3 direction =
            target.position - playerCamera.transform.position;

        Quaternion startRotation =
            transform.rotation;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                new Vector3(direction.x, 0f, direction.z)
            );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    elapsed / duration
                );

            yield return null;
        }

        transform.rotation = targetRotation;
    }
}