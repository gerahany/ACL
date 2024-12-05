using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehavior : MonoBehaviour
{
    private RandMinion campManager;
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

    public void Initialize(RandMinion manager)
    {
        campManager = manager;
    }

    public void SetAggressive(GameObject player)
    {
        isAggressive = true;
        targetPlayer = player;
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
