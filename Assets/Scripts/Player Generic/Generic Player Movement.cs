using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

void UpdateAnimations()
{
    bool isMoving = agent.remainingDistance > agent.stoppingDistance && agent.velocity.sqrMagnitude > 0.1f;

    if (animator.GetBool("isWalking") != isMoving)
    {
        animator.SetBool("isWalking", isMoving);
    }
}


}
