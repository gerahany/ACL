using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DemonBehavior : MonoBehaviour
{
    private DemonSpawner campManager;
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

    public void Initialize(DemonSpawner manager)
    {
        campManager = manager;
    }

    public void SetAggressive(GameObject player)
    {
        isAggressive = true;
        targetPlayer = player;
    }

    public void ReturnToCamp()
    {
        isAggressive = false;
        targetPlayer = null;
        agent.SetDestination(initialPosition); // Move back to the initial position
    }

    void Update()
    {
        if (isAggressive && targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.transform.position);
        }
        else if (!isAggressive && Vector3.Distance(transform.position, initialPosition) < 1f)
        {
        }
    }

    public void Die()
    {
        // Handle death logic (e.g., destroy minion, play animation)
        Destroy(gameObject);
    }

   
}
