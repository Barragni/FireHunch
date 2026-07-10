using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CompanionFollower : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private float followDistance = 2f;

    [Header("Animación")]
    [SerializeField] private string speedParameter = "Speed";

    [SerializeField] private string movingParameter = "Moving";

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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

        if (distance > followDistance)
        {
            agent.isStopped = false;

            agent.SetDestination(
                target.position
            );
        }
        else
        {
            agent.isStopped = true;
        }

        float speed =
            agent.velocity.magnitude;

        animator.SetFloat(
            speedParameter,
            speed
        );

        animator.SetBool(
            movingParameter,
            speed > 0.1f
        );
    }

    public void SetTarget(
        Transform newTarget
    )
    {
        target = newTarget;
    }
}