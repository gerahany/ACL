using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehavior2 : MonoBehaviour
{
    private RandMinion2 campManager;
    private Vector3 initialPosition;
    private bool isAggressive = false;
    private GameObject targetPlayer;
    private NavMeshAgent agent;
    private Animator animator; // Reference to Animator

    void Start()
    {
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get Animator component
    }

    public void Initialize(RandMinion2 manager)
    {
        campManager = manager;
    }

    public void SetAggressive(GameObject player)
    {
        isAggressive = true;
        targetPlayer = player;
        agent.isStopped = false;
        UpdateAnimation(true); // Set walking animation
    }

    public void ReturnToCamp()
    {
        isAggressive = false;
        targetPlayer = null;
        agent.SetDestination(initialPosition); // Move back to the initial position
        UpdateAnimation(true); // Set walking animation
    }

    void Update()
    {
        if (isAggressive && targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.transform.position);
        }
        else if (!isAggressive && Vector3.Distance(transform.position, initialPosition) < 1f)
        {
            agent.isStopped = true; // Stop moving when back at the initial position
            UpdateAnimation(false); // Set idle animation
        }
    }

    public void Die()
    {
        // Handle death logic (e.g., destroy minion, play animation)
        Destroy(gameObject);
    }

    private void UpdateAnimation(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking); // Update Animator parameter
        }
    }
}
