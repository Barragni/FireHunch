using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowerAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followDistance = 2f;

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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
            agent.SetDestination(
                target.position
            );
        }
        else
        {
            agent.ResetPath();
        }

        animator.SetBool(
            "Moving",
            agent.velocity.magnitude > 0.1f
        );
    }
}