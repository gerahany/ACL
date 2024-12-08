using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DashAbility : MonoBehaviour
{
    public float dashSpeedMultiplier = 2f; // Speed multiplier during the dash
    public float dashCooldown = 5f; // Cooldown for the dash ability
    private bool canDash = true; // Can the Rogue dash?
    private bool isSelectingPosition = false; // Is the player selecting a dash target?
    private bool isDashing = false; // Is the Rogue currently dashing?

    private NavMeshAgent agent; // Reference to the NavMeshAgent
    private Animator animator; // Reference to the Animator
    private Vector3 dashTargetPosition; // Position to dash toward

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        // Activate dash ability when pressing "Q"
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !isSelectingPosition)
        {
            StartSelectingPosition();
        }

        // If selecting position, listen for right-click to start dashing
        if (isSelectingPosition && Input.GetMouseButtonDown(1)) // Right-click
        {
            TryDash();
        }

        // Update the animator parameter to reflect the dashing state
        animator.SetBool("isDashing", isDashing);
    }

    void StartSelectingPosition()
    {
        isSelectingPosition = true;
        Debug.Log("Select a position to dash using right-click.");
    }

    void TryDash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            // Check if the target position is walkable
            if (IsWalkable(targetPosition))
            {
                dashTargetPosition = targetPosition; // Set the target position
                StartCoroutine(Dash()); // Begin the dash
            }
            else
            {
                Debug.Log("Target position is not walkable.");
            }
        }

        // Exit position selection mode
        isSelectingPosition = false;
    }

    bool IsWalkable(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            return true; // The position is walkable
        }
        return false; // The position is not walkable
    }

    IEnumerator Dash()
    {
        isDashing = true; // Start the dash
        canDash = false; // Disable further dashes during cooldown
        float originalSpeed = agent.speed; // Store the original speed
        agent.speed *= dashSpeedMultiplier; // Increase speed for dash

        Debug.Log("Dashing to target!");

        // Move the agent toward the target position
        agent.SetDestination(dashTargetPosition);

        // Wait until the agent reaches the target position
        while (Vector3.Distance(transform.position, dashTargetPosition) > agent.stoppingDistance)
        {
            yield return null; // Wait until the next frame
        }

        // Reset speed after dash
        agent.speed = originalSpeed;

        Debug.Log("Dash completed!");
        isDashing = false; // Dash ends here

        // Start cooldown after dash finishes
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Enable dashing after cooldown
        Debug.Log("Dash cooldown ended.");
    }
}
