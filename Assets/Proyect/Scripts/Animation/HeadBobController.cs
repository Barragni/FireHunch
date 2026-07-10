using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HeadBobController : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private CharacterController characterController;

    [Header("Movimiento")]
    [SerializeField] private float bobSpeed = 8f;

    [SerializeField] private float verticalAmount = 0.03f;

    [SerializeField] private float horizontalAmount = 0.015f;

    [SerializeField] private float smoothSpeed = 8f;

    [Header("Velocidad mínima")]
    [SerializeField] private float minimumSpeed = 0.1f;

    private Vector3 initialLocalPosition;

    private float timer;

    private bool headBobEnabled = true;

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
            timer = 0f;

            transform.localPosition =
                Vector3.Lerp(
                    transform.localPosition,
                    initialLocalPosition,
                    Time.deltaTime * smoothSpeed
                );

            return;
        }

        Vector3 velocity =
            characterController.velocity;

        velocity.y = 0f;

        if (velocity.magnitude < minimumSpeed)
        {
            timer = 0f;

            transform.localPosition =
                Vector3.Lerp(
                    transform.localPosition,
                    initialLocalPosition,
                    Time.deltaTime * smoothSpeed
                );

            return;
        }

        timer +=
            Time.deltaTime * bobSpeed;

        Vector3 targetPosition =
            initialLocalPosition;

        targetPosition.y +=
            Mathf.Sin(timer) *
            verticalAmount;

        targetPosition.x +=
            Mathf.Cos(timer * 0.5f) *
            horizontalAmount;

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

        if (!enabled)
        {
            timer = 0f;
        }
    }
}