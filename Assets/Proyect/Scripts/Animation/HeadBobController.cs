using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HeadBobController : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private CharacterController characterController;

    [Header("Movimiento")]
    [SerializeField] private float bobSpeed = 6.5f;

    [SerializeField] private float verticalAmount = 0.018f;

    [SerializeField] private float horizontalAmount = 0.005f;

    [SerializeField] private float smoothSpeed = 12f;

    [Header("Velocidad mínima")]
    [SerializeField] private float minimumSpeed = 0.1f;

    [Header("Transición")]
    [SerializeField] private float blendSpeed = 6f;

    private Vector3 initialLocalPosition;

    private float timer;

    private bool headBobEnabled = true;

    private float bobWeight;

    private void Start()
    {
        initialLocalPosition =
            transform.localPosition;
    }

    private void Update()
    {
        if (
            characterController == null ||
            !characterController.enabled
        )
        {
            return;
        }

        if (!headBobEnabled)
        {
            bobWeight = Mathf.Lerp(
                bobWeight,
                0f,
                Time.deltaTime * blendSpeed
            );

            ApplyBob();

            return;
        }

        Vector3 velocity =
            characterController.velocity;

        velocity.y = 0f;

        bool isMoving =
            velocity.magnitude >= minimumSpeed;

        bobWeight = Mathf.Lerp(
            bobWeight,
            isMoving ? 1f : 0f,
            Time.deltaTime * blendSpeed
        );

        if (isMoving)
        {
            timer +=
                Time.deltaTime * bobSpeed;
        }

        ApplyBob();
    }

    private void ApplyBob()
    {
        Vector3 targetPosition =
            initialLocalPosition;

        targetPosition.y +=
            Mathf.Sin(timer) *
            verticalAmount *
            bobWeight;

        targetPosition.x +=
            Mathf.Cos(timer) *
            horizontalAmount *
            bobWeight;

        transform.localPosition =
            Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * smoothSpeed
            );
    }

    public void SetEnabled(bool enabled)
    {
        headBobEnabled = enabled;
    }
}