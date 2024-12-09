using System.Collections;
using UnityEngine;

public class ArrowsAbility : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab for individual arrows
    public float arrowRainDuration = 3f; // Duration of the arrow rain
    public float arrowSpawnInterval = 0.2f; // Time between arrow spawns
    public float arrowDamageRadius = 5f; // Radius of the effect area
    public LayerMask targetLayer; // Layer for minions and demons to detect
    public int arrowDamage = 10; // Damage per arrow
    public float rotationAngle = -90f; // The angle of rotation (adjustable)

    private bool canUseAbility = true; // Can the ability be used?
    public float abilityCooldown = 10f; // Cooldown for the ability

    // Prefab for the arrow ring
    public GameObject arrowRingPrefab; // Arrow ring prefab to be used in the ability

    void Update()
    {
        // Activate the ability when pressing "E"
        if (Input.GetKeyDown(KeyCode.E) && canUseAbility)
        {
            StartSelectingPosition();
        }
    }

    void StartSelectingPosition()
    {
        Debug.Log("Select a position to use Shower of Arrows with right-click.");
        StartCoroutine(WaitForPositionSelection());
    }

    IEnumerator WaitForPositionSelection()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(1)) // Right-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPosition = hit.point;
                    Debug.Log("Selected position for Shower of Arrows: " + targetPosition);

                    // Instantiate the arrow ring prefab at the selected position
                    GameObject arrowRing = CreateArrowRing(targetPosition);

                    // Start spawning arrows
                    StartCoroutine(ActivateShowerOfArrows(targetPosition, arrowRing));
                    break;
                }
            }
            yield return null;
        }
    }

    GameObject CreateArrowRing(Vector3 targetPosition)
    {
        // Instantiate the arrow ring prefab at the target position
        GameObject arrowRing = Instantiate(arrowRingPrefab, targetPosition, Quaternion.identity);

        // Optionally, you can adjust the ring's height or position if necessary
        arrowRing.transform.position = new Vector3(targetPosition.x, targetPosition.y + 0.5f, targetPosition.z);

        return arrowRing; // Return the instantiated arrow ring so we can destroy it later
    }

    IEnumerator ActivateShowerOfArrows(Vector3 targetPosition, GameObject arrowRing)
    {
        canUseAbility = false; // Disable further use during cooldown
        float elapsedTime = 0f;

        while (elapsedTime < arrowRainDuration)
        {
            SpawnArrow(targetPosition);
            elapsedTime += arrowSpawnInterval;
            yield return new WaitForSeconds(arrowSpawnInterval);
        }

        // Destroy the arrow ring after arrows finish raining
        Destroy(arrowRing);

        // Start cooldown
        yield return new WaitForSeconds(abilityCooldown);
        canUseAbility = true;
        Debug.Log("Shower of Arrows ability is ready.");
    }

    void SpawnArrow(Vector3 targetPosition)
    {
        // Calculate random position within the ring radius
        Vector3 randomPosition = targetPosition + Random.insideUnitSphere * arrowDamageRadius;
        randomPosition.y = targetPosition.y; // Keep the y-coordinate consistent

        // Instantiate the arrow prefab at a height above the target
        GameObject arrow = Instantiate(arrowPrefab, new Vector3(randomPosition.x, targetPosition.y + 10f, randomPosition.z), Quaternion.identity);

        // Apply the desired rotation angle (we rotate the arrow by the given angle)
        arrow.transform.rotation *= Quaternion.Euler(0f, 0f, rotationAngle);

        // Add downward force or animation for the arrow
        StartCoroutine(DropArrow(arrow, targetPosition));
    }

    IEnumerator DropArrow(GameObject arrow, Vector3 targetPosition)
    {
        float fallSpeed = 10f;
        Vector3 startPosition = arrow.transform.position;

        while (arrow.transform.position.y > targetPosition.y)
        {
            arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        DealDamage(targetPosition);
        Destroy(arrow); // Destroy the arrow after it hits
    }

    void DealDamage(Vector3 hitPosition)
    {
        Collider[] hitTargets = Physics.OverlapSphere(hitPosition, arrowDamageRadius, targetLayer);
        foreach (Collider target in hitTargets)
        {
            // Apply damage to enemies
            Debug.Log("Damaged: " + target.name);
            // You can add logic here to interact with enemy health scripts
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy only if it's a valid collision
        if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion") || collision.gameObject.CompareTag("Breakable") || collision.gameObject.CompareTag("Flooring"))
        {
            Destroy(gameObject);
        }
    }
}
