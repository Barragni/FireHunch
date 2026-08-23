using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CompanionFollower : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private float followDistance = 4f;

    [Header("Velocidad")]
    [SerializeField] private float followSpeed = 6f;
    [SerializeField] private float catchUpSpeed = 8f;
    [SerializeField] private float catchUpDistance = 7f;

    [Header("Movimiento")]
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float angularSpeed = 720f;

    [Header("Tolerancia de animación")]
    [SerializeField] private float movingThreshold = 0.2f;
    [SerializeField] private float idleThreshold = 0.05f;

    [Header("Animación")]
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string movingParameter = "Moving";

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isMoving;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // El movimiento físico lo controla el NavMeshAgent.
        animator.applyRootMotion = false;

        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.stoppingDistance = followDistance;
        agent.autoBraking = false;
    }

    private void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "No se encontró un GameObject con el tag 'Player'."
            );

            enabled = false;
            return;
        }

        target = player.transform;

        agent.speed = followSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.stoppingDistance = followDistance;
        agent.autoBraking = false;
    }

    private void Update()
    {
        if (target == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        // ==========================================
        // SEGUIMIENTO
        // ==========================================

        if (distance <= followDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        else
        {
            agent.isStopped = false;

            if (distance >= catchUpDistance)
            {
                agent.speed = catchUpSpeed;
            }
            else
            {
                agent.speed = followSpeed;
            }

            agent.SetDestination(target.position);
        }

        // ==========================================
        // ANIMACIÓN
        // ==========================================

        float currentSpeed = agent.velocity.magnitude;

        animator.SetFloat(
            speedParameter,
            currentSpeed
        );

        // Histeresis:
        //
        // Para comenzar a caminar necesita superar
        // movingThreshold.
        //
        // Una vez caminando, no vuelve a Idle hasta
        // bajar de idleThreshold.
        //
        // Esto evita cambios constantes entre Walking
        // e Idle cuando la velocidad fluctúa cerca de 0.

        if (!isMoving && currentSpeed > movingThreshold)
        {
            isMoving = true;

            animator.SetBool(
                movingParameter,
                true
            );
        }
        else if (isMoving && currentSpeed < idleThreshold)
        {
            isMoving = false;

            animator.SetBool(
                movingParameter,
                false
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}