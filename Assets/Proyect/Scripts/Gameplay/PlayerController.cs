using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 150f;

    [Header("Estado inicial")]
    [SerializeField] private bool startWithMovementEnabled = false;

    [Header("Cámara de decisión")]
    [SerializeField] private Transform decisionCameraStart;

    [SerializeField] private Transform decisionCameraEnd;

    [SerializeField] private float decisionCameraDuration = 0.8f;

    [Header("Enfoque narrativo")]
    [SerializeField] private float focusRotationDuration = 0.6f;

    [Header("Zoom de diálogo")]
    [SerializeField] private bool enableDialogueZoom = true;

    [SerializeField] private float dialogueZoomFOV = 50f;

    [SerializeField] private float dialogueZoomDuration = 0.5f;

    private CharacterController characterController;
    private Camera playerCamera;
    private HeadBobController headBobController;

    private float verticalRotation;

    private bool canMove;

    private float defaultFOV;

    private void Start()
    {
        characterController =
            GetComponent<CharacterController>();

        playerCamera =
            GetComponentInChildren<Camera>();

        headBobController =
            playerCamera.GetComponent<HeadBobController>();

        defaultFOV =
            playerCamera.fieldOfView;

        SetMovementEnabled(startWithMovementEnabled);
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
        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 movement =
            transform.right * horizontal +
            transform.forward * vertical;

        characterController.Move(
            movement *
            moveSpeed *
            Time.deltaTime
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

        verticalRotation =
            Mathf.Clamp(
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
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    public void SetHeadBobEnabled(bool enabled)
    {
        if (headBobController != null)
        {
            headBobController.SetEnabled(enabled);
        }
    }

    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }

    public IEnumerator LookAtTarget(
        Transform target
    )
    {
        canMove = false;

        Vector3 direction =
            target.position -
            playerCamera.transform.position;

        Quaternion startRotation =
            transform.rotation;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                new Vector3(
                    direction.x,
                    0f,
                    direction.z
                )
            );

        float elapsed = 0f;

        while (elapsed < focusRotationDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    focusRotationDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        transform.rotation =
            targetRotation;
    }

    public IEnumerator PlayDialogueZoom()
    {
        if (!enableDialogueZoom)
        {
            yield break;
        }

        float startFOV =
            playerCamera.fieldOfView;

        float elapsed = 0f;

        while (elapsed < dialogueZoomDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    dialogueZoomDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            playerCamera.fieldOfView =
                Mathf.Lerp(
                    startFOV,
                    dialogueZoomFOV,
                    t
                );

            yield return null;
        }

        playerCamera.fieldOfView =
            dialogueZoomFOV;
    }

    public IEnumerator PlayDecisionCamera()
    {
        playerCamera.fieldOfView =
            defaultFOV;

        playerCamera.transform.position =
            decisionCameraStart.position;

        playerCamera.transform.rotation =
            decisionCameraStart.rotation;

        Coroutine effectRoutine =
            StartCoroutine(
                DecisionEffectsManager.Instance
                    .EnterDecisionMode()
            );

        float elapsed = 0f;

        while (elapsed < decisionCameraDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    decisionCameraDuration
                );

            playerCamera.transform.position =
                Vector3.Lerp(
                    decisionCameraStart.position,
                    decisionCameraEnd.position,
                    t
                );

            playerCamera.transform.rotation =
                Quaternion.Slerp(
                    decisionCameraStart.rotation,
                    decisionCameraEnd.rotation,
                    t
                );

            yield return null;
        }

        playerCamera.transform.position =
            decisionCameraEnd.position;

        playerCamera.transform.rotation =
            decisionCameraEnd.rotation;

        yield return effectRoutine;
    }
}