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

    [Header("Separación entre companions")]
    [SerializeField] private float separationDistance = 1.2f;

    [SerializeField] private float separationStrength = 1.5f;

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

    private CompanionFollower[] otherCompanions;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;

        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.stoppingDistance = followDistance;
        agent.autoBraking = false;

        // No usamos el avoidance automático de Unity,
        // porque dos companions siguiendo exactamente
        // el mismo destino pueden entrar en conflicto.
        agent.obstacleAvoidanceType =
            ObstacleAvoidanceType.NoObstacleAvoidance;
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

        agent.obstacleAvoidanceType =
            ObstacleAvoidanceType.NoObstacleAvoidance;

        // Buscar los demás companions de la escena.
        otherCompanions =
            FindObjectsByType<CompanionFollower>(
                FindObjectsSortMode.None
            );
    }

    private void Update()
    {
        if (target == null)
            return;

        float distance =
            Vector3.Distance(
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

            Vector3 destination =
                target.position;

            // Aplicar separación solamente si
            // otro companion está demasiado cerca.
            destination +=
                GetSeparationOffset();

            agent.SetDestination(
                destination
            );
        }

        // ==========================================
        // ANIMACIÓN
        // ==========================================

        float currentSpeed =
            agent.velocity.magnitude;

        animator.SetFloat(
            speedParameter,
            currentSpeed
        );

        // ==========================================
        // HISTERESIS
        // ==========================================

        if (
            !isMoving &&
            currentSpeed > movingThreshold
        )
        {
            isMoving = true;

            animator.SetBool(
                movingParameter,
                true
            );
        }
        else if (
            isMoving &&
            currentSpeed < idleThreshold
        )
        {
            isMoving = false;

            animator.SetBool(
                movingParameter,
                false
            );
        }
    }

    private Vector3 GetSeparationOffset()
    {
        if (otherCompanions == null)
            return Vector3.zero;

        Vector3 separation =
            Vector3.zero;

        foreach (
            CompanionFollower other
            in otherCompanions
        )
        {
            if (other == null || other == this)
                continue;

            Vector3 difference =
                transform.position -
                other.transform.position;

            difference.y = 0f;

            float distance =
                difference.magnitude;

            if (
                distance <= 0.001f ||
                distance >= separationDistance
            )
            {
                continue;
            }

            float strength =
                1f -
                Mathf.Clamp01(
                    distance /
                    separationDistance
                );

            separation +=
                difference.normalized *
                strength;
        }

        return separation * separationStrength;
    }

    public void SetTarget(
        Transform newTarget
    )
    {
        target = newTarget;
    }
}