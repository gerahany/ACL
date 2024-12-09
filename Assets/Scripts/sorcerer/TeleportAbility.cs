using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TeleportAbility : MonoBehaviour
{
    public float teleportCooldown = 10f; // Cooldown for the teleport ability
    private bool canTeleport = true; // Can the sorcerer teleport?
    private bool isSelectingPosition = false; // Flag to track if position is being selected
    public float maxTeleportDistance = 15f;
    private NavMeshAgent agent; // Reference to the sorcerer's NavMeshAgent (for movement)
    public ParticleSystem teleportEffectPrefab;
    public BasePlayer basePlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
    }

    void Update()
    {
        // Activate teleport ability when pressing "W"
        if (Input.GetKeyDown(KeyCode.W) && canTeleport && !isSelectingPosition && basePlayer.IsDefensiveUnlocked)
        {
            StartSelectingPosition();
        }

        // If the player is selecting a position, listen for right-click to teleport
        if (isSelectingPosition && Input.GetMouseButtonDown(1)) // Right-click (button 1)
        {
            TryTeleport();
        }
    }

    void StartSelectingPosition()
    {
        isSelectingPosition = true;
        Debug.Log("Select a position to teleport using right-click.");
    }

    void TryTeleport()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point; // Get the position where the raycast hits

            // Limit the target position to the maximum teleport distance
            Vector3 limitedPosition = LimitTeleportDistance(transform.position, targetPosition);

            // Check if the target position is walkable
            if (IsWalkable(limitedPosition))
            {
                // Start the teleportation
                Teleport(limitedPosition);
                ShowTeleportEffect();
                // Start the cooldown
                StartCoroutine(TeleportCooldown());
            }
            else
            {
                Debug.Log("Target position is not walkable.");
            }
        }

        // End the position selection
        isSelectingPosition = false;
    }

    Vector3 LimitTeleportDistance(Vector3 startPosition, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(startPosition, targetPosition);
        if (distance > maxTeleportDistance)
        {
            // Calculate the direction and set the target at the maximum distance
            Vector3 direction = (targetPosition - startPosition).normalized;
            return startPosition + direction * maxTeleportDistance;
        }
        return targetPosition; // Target is within range, no modification needed
    }

    bool IsWalkable(Vector3 targetPosition)
    {
        // Use NavMesh.SamplePosition to check if the position is walkable
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            // If the position is close enough to a walkable area
            return true;
        }
        return false; // If the position is not walkable
    }

    void Teleport(Vector3 targetPosition)
    {
        // Warp the NavMeshAgent to the target position, ensuring proper NavMesh alignment
        agent.Warp(targetPosition);

        Debug.Log("Teleport successful!");
    }

    // Coroutine for cooldown after each teleport
    IEnumerator TeleportCooldown()
    {
        canTeleport = false;
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
    void ShowTeleportEffect()
    {
        // Create the particle effect at the player's head position
        Vector3 headPosition = transform.position + Vector3.up * 2f; // Adjust this value to match the head height
        ParticleSystem effect = Instantiate(teleportEffectPrefab, headPosition, Quaternion.identity);
        
        // Play the particle effect
        effect.Play();

        // Destroy the effect after 2 seconds
        Destroy(effect.gameObject, 2f);
    }
}
