using UnityEngine;
using System.Collections;

public class RogueAbilities : MonoBehaviour
{
    public GameObject arrowPrefab; // Arrow prefab
    public Transform arrowSpawnPoint; // Point from where the arrow is spawned
    public Animator rogueAnimator; // Animator for shoot animation
    public float raycastRange = 100f; // Maximum range for detecting targets
    private Vector3 targetPosition; // Store the position of the valid target

    private bool canShootArrow = true; // Track if the arrow can be shot
    private float arrowCooldown = 1f; // Cooldown time (in seconds)

    

   void Update()
{
    // Detect right-click for shooting arrows
    if (Input.GetMouseButtonDown(1) && canShootArrow)
    {
        AttemptArrow();
    }
}

void AttemptArrow()
{
    // Perform a raycast from the camera to the mouse pointer position
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, raycastRange))
    {
        // Check if the hit object is a valid target and not the rogue itself
        if ((hit.collider.CompareTag("Demon") || hit.collider.CompareTag("Minion")) && hit.collider.gameObject != gameObject)
        {
            // Save the target position
            targetPosition = hit.point;

            // Debug log to inspect positions
            Debug.Log($"Target Position: {targetPosition}");
            Debug.Log($"Arrow Spawn Point Position: {arrowSpawnPoint.position}");

            // Calculate direction to the target and rotate the rogue
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            directionToTarget.y = 0; // Ignore vertical rotation for better control
            transform.forward = directionToTarget; // Rotate the rogue to face the target

            // Play the shoot animation
            rogueAnimator.SetTrigger("shootArrow");

            // Start the cooldown immediately
            StartCoroutine(CooldownArrow());

            // Spawn the arrow after a slight delay (adjust the timing)
            StartCoroutine(SpawnArrowWithDelay(0.6f)); // Adjust the delay as needed
        }
    }
}

private IEnumerator SpawnArrowWithDelay(float delay)
{
    // Wait for the delay to simulate a realistic arrow launch after animation
    yield return new WaitForSeconds(delay);

    // Spawn the arrow after the delay
    SpawnArrow();
}

void SpawnArrow()
{
    // Spawn the arrow at the spawn point
    GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

    // Ignore collision with the rogue
    Collider arrowCollider = arrow.GetComponent<Collider>();
    Collider rogueCollider = GetComponent<Collider>();
    if (arrowCollider != null && rogueCollider != null)
    {
        Physics.IgnoreCollision(arrowCollider, rogueCollider);
    }

    // Calculate the direction to the target (ignore the y-axis to keep it level)
    Vector3 direction = (new Vector3(targetPosition.x, arrowSpawnPoint.position.y, targetPosition.z) - arrowSpawnPoint.position).normalized;

    // Log direction to ensure it's correct
    Debug.Log($"Arrow Direction: {direction}");

    // Align the arrow's rotation to its movement direction
    Quaternion rotation = Quaternion.LookRotation(direction);

    // Apply an additional 90-degree rotation on the X-axis
    rotation *= Quaternion.Euler(0f, -90f, 0f);

    // Apply the final rotation to the arrow
    arrow.transform.rotation = rotation;

    // Set the arrow's movement direction
    arrow.GetComponent<Arrow>().SetDirection(direction);

    // Set the arrow speed (ensure speed is non-zero)
    arrow.GetComponent<Arrow>().speed = 5f;
}

IEnumerator CooldownArrow()
{
    // Disable arrow shooting during cooldown
    canShootArrow = false;
    yield return new WaitForSeconds(arrowCooldown); // Wait for the cooldown period
    canShootArrow = true; // Enable arrow shooting again
}

}
