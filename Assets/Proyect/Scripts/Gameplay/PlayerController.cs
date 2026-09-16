using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 150f;

    [Header("Estado inicial")]
    [SerializeField] private bool startWithMovementEnabled = false;

    [Header("Visual del jugador")]
    [SerializeField] private GameObject visual;

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

    [Header("Altura del jugador")]
    [SerializeField] private bool lockVerticalPosition = true;

    private CharacterController characterController;

    private Camera playerCamera;

    private HeadBobController headBobController;

    private float verticalRotation;

    private bool canMove;

    private bool wasWalking;

    private float defaultFOV;

    // Altura que tendrá el jugador mientras esté bloqueada
    private float lockedY;


    // ============================================================
    // INICIALIZACIÓN
    // ============================================================

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController>();

        playerCamera =
            GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            headBobController =
                playerCamera.GetComponent<HeadBobController>();

            defaultFOV =
                playerCamera.fieldOfView;
        }

        // Guardamos la altura inicial del jugador
        lockedY =
            transform.position.y;

        SetMovementEnabled(
            startWithMovementEnabled
        );

        HideVisual();

        ResetFootsteps();
    }


    private void Update()
    {
        if (!canMove)
        {
            ResetFootsteps();
            return;
        }

        HandleMouseLook();

        HandleMovement();

        HandleFootsteps();

        MaintainLockedHeight();
    }


    // ============================================================
    // MOUSE LOOK
    // ============================================================

    private void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            return;
        }

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


    // ============================================================
    // MOVIMIENTO
    // ============================================================

    private void HandleMovement()
    {
        if (characterController == null)
        {
            return;
        }

        float horizontal =
            Input.GetAxis("Horizontal");

        float vertical =
            Input.GetAxis("Vertical");

        Vector3 movement =
            transform.right * horizontal +
            transform.forward * vertical;

        // El movimiento del jugador SIEMPRE es horizontal.
        // Nunca enviamos movimiento vertical al CharacterController.
        movement.y = 0f;

        characterController.Move(
            movement *
            moveSpeed *
            Time.deltaTime
        );
    }


    // ============================================================
    // BLOQUEO DE ALTURA
    // ============================================================

    private void MaintainLockedHeight()
    {
        if (!lockVerticalPosition)
        {
            return;
        }

        Vector3 position =
            transform.position;

        position.y =
            lockedY;

        transform.position =
            position;
    }


    // ============================================================
    // PASOS
    // ============================================================

    private void HandleFootsteps()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        bool isWalking =
            Mathf.Abs(horizontal) > 0.01f ||
            Mathf.Abs(vertical) > 0.01f;

        // --------------------------------------------------------
        // COMENZÓ A MOVERSE
        // --------------------------------------------------------

        if (
            isWalking &&
            !wasWalking
        )
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFootstep();
            }
        }

        // --------------------------------------------------------
        // DEJÓ DE MOVERSE
        // --------------------------------------------------------

        if (
            !isWalking &&
            wasWalking
        )
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopFootstep();
            }
        }

        wasWalking =
            isWalking;
    }


    private void ResetFootsteps()
    {
        wasWalking = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
        }
    }


    // ============================================================
    // MOVIMIENTO ENABLE / DISABLE
    // ============================================================

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            ResetFootsteps();
        }

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


    // ============================================================
    // HEAD BOB
    // ============================================================

    public void SetHeadBobEnabled(bool enabled)
    {
        if (headBobController != null)
        {
            headBobController.SetEnabled(enabled);
        }
    }


    // ============================================================
    // VISUAL
    // ============================================================

    public void ShowVisual()
    {
        if (visual != null)
        {
            visual.SetActive(true);
        }
    }


    public void HideVisual()
    {
        if (visual != null)
        {
            visual.SetActive(false);
        }
    }


    // ============================================================
    // CÁMARA
    // ============================================================

    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }


    // ============================================================
    // MIRAR OBJETIVO
    // ============================================================

    public IEnumerator LookAtTarget(
        Transform target
    )
    {
        if (playerCamera == null || target == null)
        {
            yield break;
        }

        canMove = false;

        ResetFootsteps();

        Vector3 direction =
            (
                target.position -
                playerCamera.transform.position
            ).normalized;

        float targetYaw =
            Mathf.Atan2(
                direction.x,
                direction.z
            ) *
            Mathf.Rad2Deg;

        float targetPitch =
            -Mathf.Asin(direction.y) *
            Mathf.Rad2Deg;

        targetPitch =
            Mathf.Clamp(
                targetPitch,
                -80f,
                80f
            );

        Quaternion startBodyRotation =
            transform.rotation;

        Quaternion endBodyRotation =
            Quaternion.Euler(
                0f,
                targetYaw,
                0f
            );

        float startPitch =
            verticalRotation;

        float elapsed = 0f;

        while (
            elapsed <
            focusRotationDuration
        )
        {
            elapsed +=
                Time.deltaTime;

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
                    startBodyRotation,
                    endBodyRotation,
                    t
                );

            verticalRotation =
                Mathf.Lerp(
                    startPitch,
                    targetPitch,
                    t
                );

            playerCamera.transform.localRotation =
                Quaternion.Euler(
                    verticalRotation,
                    0f,
                    0f
                );

            yield return null;
        }

        transform.rotation =
            endBodyRotation;

        verticalRotation =
            targetPitch;

        playerCamera.transform.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );
    }


    // ============================================================
    // ZOOM DE DIÁLOGO
    // ============================================================

    public IEnumerator PlayDialogueZoom()
    {
        if (!enableDialogueZoom)
        {
            yield break;
        }

        if (playerCamera == null)
        {
            yield break;
        }

        float startFOV =
            playerCamera.fieldOfView;

        float elapsed = 0f;

        while (
            elapsed <
            dialogueZoomDuration
        )
        {
            elapsed +=
                Time.deltaTime;

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


    // ============================================================
    // CÁMARA DE DECISIÓN
    // ============================================================

    public IEnumerator PlayDecisionCamera()
    {
        if (playerCamera == null)
        {
            yield break;
        }

        if (
            decisionCameraStart == null ||
            decisionCameraEnd == null
        )
        {
            yield break;
        }

        ShowVisual();

        ResetFootsteps();

        playerCamera.fieldOfView =
            defaultFOV;

        playerCamera.transform.position =
            decisionCameraStart.position;

        playerCamera.transform.rotation =
            decisionCameraStart.rotation;

        if (DecisionEffectsManager.Instance != null)
        {
            Coroutine effectRoutine =
                StartCoroutine(
                    DecisionEffectsManager.Instance
                        .EnterDecisionMode()
                );

            float elapsed = 0f;

            while (
                elapsed <
                decisionCameraDuration
            )
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
        else
        {
            float elapsed = 0f;

            while (
                elapsed <
                decisionCameraDuration
            )
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
        }
    }
}