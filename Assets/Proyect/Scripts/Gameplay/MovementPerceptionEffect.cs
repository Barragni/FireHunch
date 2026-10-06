using UnityEngine;

public class MovementPerceptionEffect : MonoBehaviour
{
    [Header("Cámara")]
    [SerializeField] private Camera playerCamera;

    [Header("Efecto")]
    [SerializeField] private float backwardOffset = 0.03f;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Detección")]
    [SerializeField] private float minimumMovement = 0.1f;

    private CharacterController characterController;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
            originalLocalPosition = playerCamera.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (playerCamera == null || characterController == null)
            return;

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;

        bool isMoving = velocity.magnitude > minimumMovement;

        Vector3 targetPosition = originalLocalPosition;

        if (isMoving)
        {
            targetPosition -= Vector3.forward * backwardOffset;
        }

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }
}